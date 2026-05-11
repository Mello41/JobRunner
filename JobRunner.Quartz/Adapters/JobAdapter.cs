using JobRunner.Core.DefaultImplementations;
using JobRunner.Core.Events;
using JobRunner.Core.Events.TaskEvents.TaskStatus;
using JobRunner.Core.Interfaces.Core;
using JobRunner.Core.Interfaces.EntityServices;
using JobRunner.Core.Interfaces.Execution;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Collections.Concurrent;

namespace JobRunner.Quartz.Adapters
{
    /// <summary>
    /// паттерн Adapter, который соединяет 
    /// доменную модель с внешним планировщиком Quartz.NET
    /// </summary>
    public class JobAdapter : IJob
    {
        private readonly ITaskService<JobTask> _storage;
        private readonly IJobExecutor _executor;
        private readonly IDomainEventDispatcher _dispatcher;
        private readonly IEncryptionService _encryption;
        private readonly ILogger<JobAdapter> _logger;

        /// <summary>
        /// Для блокировки race condition
        /// </summary>
        private static readonly ConcurrentDictionary<Guid, SemaphoreSlim> _locks = new();

        public JobAdapter(
            ITaskService<JobTask> storage,
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

        public async Task Execute(IJobExecutionContext context)
        {
            var taskIdStr = context.MergedJobDataMap.GetString("TaskId");
            var taskId = Guid.Parse(taskIdStr);

            var semaphore = _locks.GetOrAdd(taskId, new SemaphoreSlim(1, 1));

            await semaphore.WaitAsync(context.CancellationToken);

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

        private async Task ExecuteInternal(Guid taskId, CancellationToken cancellationToken)
        {
            var task = await _storage.GetByIdAsync(taskId);
            if (task == null)
            {
                _logger.LogWarning("Task {TaskId} not found", taskId);
                return;
            }

            if (!task.AllowConcurrentExecution && task.JobTaskMetadata.IsRunning)
            {
                _logger.LogWarning(
                    "Task {TaskName} (Id: {TaskId}) is already running and concurrent execution is disabled",
                    task.Name, task.Id);
                return;
            }

            await _encryption.DecryptSensitiveArgumentsAsync(task.ScheduleArguments, cancellationToken);

            try
            {
                await _dispatcher.PublishAsync(new TaskStartedEvent
                {
                    TaskId = task.Id,
                    TaskName = task.Name,
                    StartTime = DateTime.UtcNow,
                    ExecutionPath = task.ExecutionPath,
                    NotifySettings = task.NotifySettings 
                }, cancellationToken);

                task.JobTaskMetadata.IsRunning = true;
                task.JobTaskMetadata.LastRun = DateTime.UtcNow;
                task.StartRun = DateTime.UtcNow;
                await _storage.UpdateAsync(task);

                var result = await _executor.ExecuteAsync(task, cancellationToken);

                task.JobTaskMetadata.LastRun = result.StartTime;
                task.JobTaskMetadata.LastDurationMs = result.DurationMs;
                task.JobTaskMetadata.TotalRunCount++;

                if (result.Success)
                {
                    task.JobTaskMetadata.SuccessCount++;
                    task.JobTaskMetadata.ConsecutiveFailures = 0;  
                }
                else
                {
                    task.JobTaskMetadata.FailureCount++;
                    task.JobTaskMetadata.ConsecutiveFailures++;    
                    task.JobTaskMetadata.LastError = result.ErrorMessage;
                    task.JobTaskMetadata.LastErrorTime = DateTime.UtcNow;
                }

                task.JobTaskMetadata.IsRunning = false;
                task.JobTaskMetadata.TaskPID = result.ProcessId;
                task.EndRun = result.EndTime ?? DateTime.UtcNow;

                await _storage.UpdateAsync(task);

                await _dispatcher.PublishAsync(new TaskCompletedEvent
                {
                    TaskId = task.Id,
                    TaskName = task.Name,
                    Success = result.Success,
                    ErrorMessage = result.ErrorMessage,
                    CompletionTime = result.EndTime ?? DateTime.UtcNow,
                    DurationMs = result.DurationMs ?? 0,
                    NotifySettings = task.NotifySettings 
                }, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing task {TaskId}", task.Id);

                task.JobTaskMetadata.IsRunning = false;
                task.JobTaskMetadata.FailureCount++;
                task.JobTaskMetadata.ConsecutiveFailures++;
                task.JobTaskMetadata.LastError = ex.Message;
                task.JobTaskMetadata.LastErrorTime = DateTime.UtcNow;
                task.JobTaskMetadata.IsCompleted = false;

                await _storage.UpdateAsync(task);

                await _dispatcher.PublishAsync(new TaskCompletedEvent
                {
                    TaskId = task.Id,
                    TaskName = task.Name,
                    Success = false,
                    ErrorMessage = ex.Message,
                    CompletionTime = DateTime.UtcNow,
                    DurationMs = 0,
                    NotifySettings = task.NotifySettings
                }, cancellationToken);
            }
            finally
            {
                await _encryption.ReencryptSensitiveArgumentsAsync(task.ScheduleArguments, cancellationToken);
                await _storage.UpdateAsync(task);
            }
        }
    }
}