using JobRunner.Core.DefaultImplementations;
using JobRunner.Core.Entities;
using JobRunner.Core.Events;
using JobRunner.Core.Interfaces.Core;
using JobRunner.Core.Interfaces.EntityServices;
using JobRunner.Core.Interfaces.Execution;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Collections.Concurrent;

namespace JobRunner.Quartz.Adapters
{
    /// <summary>
    /// паттерн Adapter, который соединяет (адаптирует)
    /// доменную модель с внешним планировщиком Quartz.NET
    /// </summary>
    public class JobAdapter : IJob
    {
        private readonly ITaskService<IJobTask> _storage;
        private readonly IJobExecutor _executor;
        private readonly IDomainEventDispatcher _dispatcher;
        private readonly IEncryptionService _encryption;
        private readonly ILogger<JobAdapter> _logger;

        /// <summary>
        /// Для блокировки race condition
        /// </summary>
        private static readonly ConcurrentDictionary<Guid, SemaphoreSlim> _locks = new();

        public JobAdapter(
            ITaskService<IJobTask> storage,
            IJobExecutor executor,
            IDomainEventDispatcher dispatcher,
            IEncryptionService encryption,
            ILogger<JobAdapter> logger)
        {
            _storage = storage;
            _executor = executor;
            _dispatcher = dispatcher;
            _encryption = encryption;
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Execute(IJobExecutionContext context)
        {
            var taskIdStr = context.MergedJobDataMap.GetString("TaskId");
            var taskId = Guid.Parse(taskIdStr);

            var semaphore = _locks.GetOrAdd(taskId, new SemaphoreSlim(1, 1));

            if (!semaphore.Wait(0))
            {
                _logger.LogWarning(
                    "Task {TaskId} is already running, skipping this trigger. " +
                    "Consider increasing interval or enabling AllowConcurrentExecution",
                    taskId);

                if (semaphore.CurrentCount == 1)
                    _locks.TryRemove(taskId, out _);

                return;
            }

            try
            {
                await ExecuteInternal(taskId, context.CancellationToken);
            }
            finally
            {
                semaphore.Release();
                if (semaphore.CurrentCount == 1)
                    _locks.TryRemove(taskId, out _);
            }
        }

        /// <summary>
        /// Выполняет внутреннюю логику запуска задачи.
        /// </summary>
        /// <param name="taskId">Идентификатор задачи</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Task, представляющий асинхронную операцию</returns>
        /// <remarks>
        /// Метод координирует полный жизненный цикл выполнения задачи:
        /// загрузка, валидация, расшифровка аргументов, публикация событий,
        /// выполнение, обновление статистики, повторное шифрование.
        /// В случае ошибки обновляет метаданные и публикует событие об ошибке.
        /// </remarks>
        private async Task ExecuteInternal(Guid taskId, CancellationToken cancellationToken)
        {
            var task = await LoadAndValidateTaskAsync(taskId, cancellationToken);
            if (task == null) return;

            var executionScope = new ExecutionScope(task, _logger);

            try
            {
                await executionScope.DecryptArgumentsAsync(_encryption, cancellationToken);
                await executionScope.PublishStartedEventAsync(_dispatcher, cancellationToken);
                await executionScope.UpdateBeforeExecutionAsync(_storage, cancellationToken);

                var result = await _executor.ExecuteAsync(task, cancellationToken);

                await executionScope.UpdateAfterExecutionAsync(result, _storage, cancellationToken);
                await executionScope.PublishCompletedEventAsync(result, _dispatcher, cancellationToken);
            }
            catch (OperationCanceledException ex)
            {
                await executionScope.HandleCancellationAsync(ex, _storage, _dispatcher, cancellationToken);
                throw;
            }
            catch (Exception ex)
            {
                await executionScope.HandleFailureAsync(ex, _storage, _dispatcher, cancellationToken);
                throw;
            }
            finally
            {
                await executionScope.ReencryptArgumentsAsync(_encryption, cancellationToken);
            }
        }

        /// <summary>
        /// Загружает задачу из хранилища и проверяет возможность выполнения.
        /// </summary>
        /// <param name="taskId">Идентификатор задачи</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Задача или null, если выполнение невозможно</returns>
        /// <remarks>
        /// Проверяет: существование задачи, активность (IsEnabled),
        /// возможность параллельного выполнения (AllowConcurrentExecution).
        /// При недоступности задачи логирует причину пропуска.
        /// </remarks>
        private async Task<IJobTask?> LoadAndValidateTaskAsync(Guid taskId, CancellationToken cancellationToken)
        {
            var task = await _storage.GetByIdAsync(taskId, cancellationToken);

            if (task == null)
            {
                _logger.LogWarning("Task {TaskId} not found in storage. Skipping execution.", taskId);
                return null;
            }

            if (!task.IsEnabled)
            {
                _logger.LogInformation(
                    "Task {TaskName} (Id: {TaskId}) is disabled. Skipping scheduled execution.",
                    task.Name, task.Id);
                return null;
            }

            if (!task.AllowConcurrentExecution && task.JobTaskMetadata.IsRunning)
            {
                _logger.LogWarning(
                    "Task {TaskName} (Id: {TaskId}) is already running and concurrent execution is disabled. " +
                    "Skipping this trigger. Consider increasing execution interval or enabling AllowConcurrentExecution.",
                    task.Name, task.Id);
                return null;
            }

            return task;
        }
    }
}