using JobRunner.Core.Interfaces.Core;
using JobRunner.Core.Interfaces.Entities;
using JobRunner.Core.Interfaces.Entities.EntityServices;
using JobRunner.Core.Interfaces.Events.DomainEvent;
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
    public class JobAdapter<TTask, TId> : IJob
                        where TTask : class, IJobTask<TId>
                        where TId : IEquatable<TId>
    {
        private readonly IJobTaskService<TTask, TId> _storage;
        private readonly IJobExecutor<TId> _executor;
        private readonly IDomainEventDispatcher _dispatcher;
        private readonly IEncryptionService _encryption;
        private readonly ILogger<JobAdapter<TTask, TId>> _logger;

        /// <summary>
        /// Для блокировки race condition
        /// </summary>
        private static readonly ConcurrentDictionary<TId, SemaphoreSlim> _locks = new();

        public JobAdapter(
            IJobTaskService<TTask, TId> storage,
            IJobExecutor<TId> executor,
            IDomainEventDispatcher dispatcher,
            IEncryptionService encryption,
            ILogger<JobAdapter<TTask, TId>> logger)
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
            var taskId = ParseId(taskIdStr);

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
        private async Task ExecuteInternal(TId taskId, CancellationToken cancellationToken)
        {
            var task = await LoadAndValidateTaskAsync(taskId, cancellationToken);
            if (task is null) return;

            var executionScope = new ExecutionScope<TTask, TId>(task, _logger, _dispatcher);

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
        private async Task<TTask?> LoadAndValidateTaskAsync(TId taskId, CancellationToken cancellationToken)
        {
            var task = await _storage.GetByIdAsync(taskId, cancellationToken);

            if (task is null)
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

        /// <summary>
        /// Преобразует строку из JobDataMap в TId
        /// </summary>
        /// <param name="idStr">Строковое представление ID из JobDataMap</param>
        /// <returns>Распаршенный ID типа TId</returns>
        /// <exception cref="InvalidOperationException">Когда ID имеет неверный формат или тип не поддерживается</exception>
        private TId ParseId(string idStr)
        {
            if (string.IsNullOrEmpty(idStr))
                throw new InvalidOperationException("TaskId cannot be null or empty in JobDataMap");

            try
            {
                if (typeof(TId) == typeof(Guid))
                {
                    if (Guid.TryParse(idStr, out var guid))
                        return (TId)(object)guid;

                    throw new InvalidOperationException($"Invalid Guid format for TaskId: {idStr}");
                }

                if (typeof(TId) == typeof(long))
                {
                    if (long.TryParse(idStr, out var longId))
                        return (TId)(object)longId;

                    throw new InvalidOperationException($"Invalid Int64 format for TaskId: {idStr}");
                }

                if (typeof(TId) == typeof(int))
                {
                    if (int.TryParse(idStr, out var intId))
                        return (TId)(object)intId;

                    throw new InvalidOperationException($"Invalid Int32 format for TaskId: {idStr}");
                }

                if (typeof(TId) == typeof(string))
                    return (TId)(object)idStr;

                throw new NotSupportedException($"Unsupported ID type: {typeof(TId)}. Supported types: Guid, long, int, string");
            }
            catch (Exception ex) when (ex is not InvalidOperationException and not NotSupportedException)
            {
                throw new InvalidOperationException($"Failed to parse TaskId '{idStr}' to type {typeof(TId)}", ex);
            }
        }
    }
}