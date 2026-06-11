using JobRunner.Core.Events.TaskEvents.TaskHistory;
using JobRunner.Core.Events.TaskEvents.TaskStatus;
using JobRunner.Core.Interfaces;
using JobRunner.Core.Interfaces.Core;
using JobRunner.Core.Interfaces.Entities;
using JobRunner.Core.Interfaces.Entities.EntityServices;
using JobRunner.Core.Interfaces.Events.DomainEvent;
using JobRunner.Core.Results;
using Microsoft.Extensions.Logging;

namespace JobRunner.Quartz
{
    /// <summary>
    /// Реализация IExecutionScope для Quartz с поддержкой любого типа, реализующего IJobTask.
    /// Инкапсулирует состояние выполнения задачи и управляет жизненным циклом: расшифровка аргументов,
    /// публикация событий, обновление метаданных, повторное шифрование.
    /// </summary>
    public class ExecutionScope<TTask, TId> : IExecutionScope<TTask, TId>
                                        where TTask : class, IJobTask<TId>
                                        where TId : IEquatable<TId>
    {
        private readonly ILogger _logger;
        private readonly IDomainEventDispatcher _dispatcher;
        private readonly TTask _task;
        private bool _decrypted = false;

        /// <summary>
        /// Возвращает задачу, которая выполняется в текущем scope.
        /// </summary>
        public TTask Task => _task;

        /// <summary>
        /// Время старта выполнения задачи в формате UTC.
        /// </summary>
        public DateTime StartTime { get; private set; }

        public ExecutionScope(TTask task, ILogger logger, IDomainEventDispatcher dispatcher)
        {
            _task = task ?? throw new ArgumentNullException(nameof(task));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        }

        /// <summary>
        /// Расшифровывает чувствительные аргументы задачи через сервис шифрования.
        /// Устанавливает внутренний флаг _decrypted для отслеживания состояния.
        /// Должен вызываться до начала выполнения задачи.
        /// </summary>
        /// <param name="encryption"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task DecryptArgumentsAsync(IEncryptionService encryption, CancellationToken cancellationToken = default)
        {
            await encryption.DecryptSensitiveArgumentsAsync(_task.ScheduleArguments, cancellationToken);
            _decrypted = true;
            _logger.LogDebug("Sensitive arguments decrypted for task {TaskName}", _task.Name);
        }

        /// <summary>
        /// Публикует событие TaskStartedEvent через диспетчер доменных событий.
        /// Устанавливает StartTime в текущее время UTC. Вызывается после расшифровки аргументов.
        /// Событие используется для уведомлений, логирования и аудита.
        /// </summary>
        public async Task PublishStartedEventAsync(IDomainEventDispatcher dispatcher, CancellationToken cancellationToken = default)
        {
            StartTime = DateTime.UtcNow;

            await dispatcher.PublishAsync(new TaskStartedEvent<TId>
            {
                TaskId = _task.Id,
                TaskName = _task.Name,
                StartTime = StartTime,
                ExecutionPath = _task.ExecutionPath,
                NotifySettings = _task.NotifySettings
            }, cancellationToken);

            _logger.LogInformation("Task {TaskName} started at {StartTime}", _task.Name, StartTime);
        }

        /// <summary>
        /// Обновляет метаданные задачи перед выполнением: устанавливает IsRunning = true,
        /// LastRun и StartRun равными StartTime. Сохраняет изменения через сервис хранения.
        /// Вызывается после публикации события старта.
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UpdateBeforeExecutionAsync(IJobTaskService<TTask, TId> storage, CancellationToken cancellationToken = default)
        {
            _task.JobTaskMetadata.IsRunning = true;
            _task.JobTaskMetadata.LastRun = StartTime;
            _task.StartRun = StartTime;

            await storage.UpdateAsync(_task, cancellationToken);
            _logger.LogDebug("Task metadata updated before execution");
        }

        /// <summary>
        /// Обновляет метаданные задачи после выполнения: TotalRunCount, SuccessCount/FailureCount,
        /// длительность выполнения, PID процесса, сбрасывает IsRunning = false.
        /// При успехе обнуляет ConsecutiveFailures, при ошибке увеличивает счётчик и сохраняет ошибку.
        /// Сохраняет изменения через сервис хранения.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="storage"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UpdateAfterExecutionAsync(JobExecutionResult result, IJobTaskService<TTask, TId> storage, CancellationToken cancellationToken = default)
        {
            var endTime = result.EndTime ?? DateTime.UtcNow;
            var durationMs = result.DurationMs ?? (long)(endTime - StartTime).TotalMilliseconds;

            _task.JobTaskMetadata.LastDurationMs = durationMs;
            _task.JobTaskMetadata.TotalRunCount++;

            if (result.Success)
            {
                _task.JobTaskMetadata.SuccessCount++;
                _task.JobTaskMetadata.ConsecutiveFailures = 0;
                _logger.LogInformation("Task {TaskName} completed successfully in {DurationMs}ms", _task.Name, durationMs);
            }
            else
            {
                _task.JobTaskMetadata.FailureCount++;
                _task.JobTaskMetadata.ConsecutiveFailures++;
                _task.JobTaskMetadata.LastError = result.ErrorMessage;
                _task.JobTaskMetadata.LastErrorTime = DateTime.UtcNow;
                _logger.LogError("Task {TaskName} failed. Execution details: {@ExecutionResult}",
                                _task.Name, new { result.ExitCode, result.IsTimeout, result.StartTime });
            }

            _task.JobTaskMetadata.IsRunning = false;

            if (result.ProcessId.HasValue)
            {
                _task.JobTaskMetadata.TaskPID = result.ProcessId.Value;
            }

            _task.EndRun = endTime;
            await storage.UpdateAsync(_task, cancellationToken);

            await PublishHistoryEventAsync(result, result.Success ? "completed" : "failed", cancellationToken);
        }

        /// <summary>
        /// Публикует событие TaskCompletedEvent через диспетчер доменных событий.
        /// Содержит информацию об успешности выполнения, ошибке и длительности.
        /// Вызывается после обновления метаданных задачи.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="dispatcher"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task PublishCompletedEventAsync(JobExecutionResult result, IDomainEventDispatcher dispatcher, CancellationToken cancellationToken = default)
        {
            var endTime = result.EndTime ?? DateTime.UtcNow;
            var durationMs = result.DurationMs ?? (long)(endTime - StartTime).TotalMilliseconds;

            await dispatcher.PublishAsync(new TaskCompletedEvent<TId>
            {
                TaskId = _task.Id,
                TaskName = _task.Name,
                Success = result.Success,
                ErrorMessage = result.ErrorMessage,
                CompletionTime = endTime,
                DurationMs = durationMs,
                NotifySettings = _task.NotifySettings
            }, cancellationToken);
        }

        /// <summary>
        /// Обрабатывает отмену выполнения задачи (OperationCanceledException).
        /// Обновляет метаданные: сбрасывает IsRunning, увеличивает FailureCount и ConsecutiveFailures,
        /// сохраняет сообщение об отмене. Публикует событие TaskCompletedEvent с флагом Success = false.
        /// Вызывается при явной отмене через CancellationToken.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="storage"></param>
        /// <param name="dispatcher"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task HandleCancellationAsync(OperationCanceledException ex, IJobTaskService<TTask, TId> storage,
            IDomainEventDispatcher dispatcher, CancellationToken cancellationToken = default)
        {
            _logger.LogWarning(ex, "Task {TaskName} execution was cancelled", _task.Name);

            _task.JobTaskMetadata.IsRunning = false;
            _task.JobTaskMetadata.FailureCount++;
            _task.JobTaskMetadata.ConsecutiveFailures++;
            _task.JobTaskMetadata.LastError = "Execution was cancelled";
            _task.JobTaskMetadata.LastErrorTime = DateTime.UtcNow;

            await storage.UpdateAsync(_task, cancellationToken);

            await dispatcher.PublishAsync(new TaskCompletedEvent<TId>
            {
                TaskId = _task.Id,
                TaskName = _task.Name,
                Success = false,
                ErrorMessage = "Execution was cancelled",
                CompletionTime = DateTime.UtcNow,
                DurationMs = 0,
                NotifySettings = _task.NotifySettings
            }, cancellationToken);

            var emptyResult = new JobExecutionResult { Success = false, ErrorMessage = "Execution was cancelled" };
            await PublishHistoryEventAsync(emptyResult, "cancelled", cancellationToken);
        }

        /// <summary>
        /// Обрабатывает непредвиденную ошибку выполнения задачи (любое исключение кроме OperationCanceledException).
        /// Обновляет метаданные: сбрасывает IsRunning, увеличивает FailureCount и ConsecutiveFailures,
        /// сохраняет сообщение об ошибке. Публикует событие TaskCompletedEvent с флагом Success = false.
        /// Вызывается при любом исключении в процессе выполнения.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="storage"></param>
        /// <param name="dispatcher"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task HandleFailureAsync(Exception ex, IJobTaskService<TTask, TId> storage,
            IDomainEventDispatcher dispatcher, CancellationToken cancellationToken = default)
        {
            _logger.LogError(ex, "Unexpected error executing task {TaskName}", _task.Name);

            _task.JobTaskMetadata.IsRunning = false;
            _task.JobTaskMetadata.FailureCount++;
            _task.JobTaskMetadata.ConsecutiveFailures++;
            _task.JobTaskMetadata.LastError = ex.Message;
            _task.JobTaskMetadata.LastErrorTime = DateTime.UtcNow;

            await storage.UpdateAsync(_task, cancellationToken);

            await dispatcher.PublishAsync(new TaskCompletedEvent<TId>
            {
                TaskId = _task.Id,
                TaskName = _task.Name,
                Success = false,
                ErrorMessage = ex.Message,
                CompletionTime = DateTime.UtcNow,
                DurationMs = 0,
                NotifySettings = _task.NotifySettings
            }, cancellationToken);

            var emptyResult = new JobExecutionResult { Success = false, ErrorMessage = ex.Message };
            await PublishHistoryEventAsync(emptyResult, "failed", cancellationToken);
        }

        /// <summary>
        /// Повторно шифрует чувствительные аргументы задачи через сервис шифрования.
        /// Выполняется только если ранее была вызвана расшифровка (флаг _decrypted = true).
        /// ВЫЗЫВАЕТСЯ ВСЕГДА в блоке finally для защиты от утечек данных.
        /// При ошибке шифрования логирует CRITICAL, но не пробрасывает исключение,
        /// чтобы не потерять основную ошибку выполнения. Данные могут остаться незашифрованными.
        /// </summary>
        /// <param name="encryption"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task ReencryptArgumentsAsync(IEncryptionService encryption, CancellationToken cancellationToken = default)
        {
            if (!_decrypted) return;

            try
            {
                await encryption.ReencryptSensitiveArgumentsAsync(_task.ScheduleArguments, cancellationToken);
                _decrypted = false;
                _logger.LogDebug("Sensitive arguments re-encrypted for task {TaskName}", _task.Name);
            }
            catch (Exception encryptionEx)
            {
                _logger.LogCritical(encryptionEx, "CRITICAL: Failed to re-encrypt arguments for task {TaskName}", _task.Name);
            }
        }

        /// <summary>
        /// Публикует событие истории выполнения задачи.
        /// Содержит полную информацию о запуске и результате для аудита и анализа.
        /// </summary>
        /// <param name="result">Результат выполнения задачи</param>
        /// <param name="status">Статус выполнения (completed, failed, cancelled, timeout)</param>
        /// <param name="ct">Токен отмены</param>
        private async Task PublishHistoryEventAsync(JobExecutionResult result, string status, CancellationToken ct)
        {
            await _dispatcher.PublishAsync(new TaskHistoryEvent<TId>
            {
                TaskId = _task.Id,
                TaskName = _task.Name,
                StartTime = StartTime,
                EndTime = result.EndTime ?? DateTime.UtcNow,
                DurationMs = result.DurationMs,
                Success = result.Success,
                Status = status,
                ErrorMessage = result.ErrorMessage,
                ProcessId = result.ProcessId,
                ExitCode = result.ExitCode,
                TriggeredBy = "schedule"
            }, ct);
        }
    }
}