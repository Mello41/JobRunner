using JobRunner.Core.DefaultImplementations;
using JobRunner.Core.Interfaces.EntityServices;
using JobRunner.Core.Interfaces.Execution;
using JobRunner.Core.Events;                
using JobRunner.Core.Events.TaskEvents.TaskStatus;
using Quartz;

namespace JobRunner.Quartz.Adapters
{
    public class JobAdapter : IJob
    {
        private readonly ITaskService<JobTask> _storage;
        private readonly IJobExecutor _executor;           
        private readonly IDomainEventDispatcher _dispatcher; 

        public JobAdapter(
            ITaskService<JobTask> storage,
            IJobExecutor executor,              
            IDomainEventDispatcher dispatcher) 
        {
            _storage = storage;
            _executor = executor;
            _dispatcher = dispatcher;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var taskIdStr = context.MergedJobDataMap.GetString("TaskId");
            var taskId = Guid.Parse(taskIdStr);

            var task = await _storage.GetByIdAsync(taskId);
            if (task == null) return;

            await _dispatcher.PublishAsync(new TaskStartedEvent
            {
                TaskId = task.Id,
                TaskName = task.Name,
                StartTime = DateTime.UtcNow,
                ExecutionPath = task.ExecutionPath
            }, context.CancellationToken);

            var result = await _executor.ExecuteAsync(task, context.CancellationToken);

            await _dispatcher.PublishAsync(new TaskCompletedEvent
            {
                TaskId = task.Id,
                TaskName = task.Name,
                Success = result.Success,
                ErrorMessage = result.ErrorMessage,
                CompletionTime = result.EndTime ?? DateTime.UtcNow,
                DurationMs = result.DurationMs ?? 0
            }, context.CancellationToken);

            task.JobTaskMetadata.LastRun = result.StartTime;
            task.JobTaskMetadata.LastDurationMs = result.DurationMs;
            task.JobTaskMetadata.TotalRunCount++;
            if (result.Success) task.JobTaskMetadata.SuccessCount++;
            else task.JobTaskMetadata.FailureCount++;
            task.JobTaskMetadata.IsRunning = false;
            task.JobTaskMetadata.TaskPID = result.ProcessId;

            await _storage.UpdateAsync(task);
        }
    }
}