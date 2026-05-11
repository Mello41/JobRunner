using System.Threading;
using System.Threading.Tasks;
using JobRunner.Core.Events.TaskEvents.TaskStatus;
using Microsoft.Extensions.Logging;

namespace JobRunner.Core.Events.Handlers
{
    /// <summary>
    /// Логирование завершения задачи
    /// </summary>
    public class LogTaskCompletedHandler : IDomainEventHandler<TaskCompletedEvent>
    {
        private readonly ILogger<LogTaskCompletedHandler> _logger;

        public LogTaskCompletedHandler(ILogger<LogTaskCompletedHandler> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(TaskCompletedEvent @event, CancellationToken cancellationToken)
        {
            if (@event.Success)
            {
                _logger.LogInformation(
                    "Task {TaskName} (Id: {TaskId}) completed successfully in {DurationMs} ms",
                    @event.TaskName, @event.TaskId, @event.DurationMs);
            }
            else
            {
                _logger.LogError(
                    "Task {TaskName} (Id: {TaskId}) failed: {ErrorMessage}",
                    @event.TaskName, @event.TaskId, @event.ErrorMessage);
            }

            return Task.CompletedTask;
        }
    }
}
