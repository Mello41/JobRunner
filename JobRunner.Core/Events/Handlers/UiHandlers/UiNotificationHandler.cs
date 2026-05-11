using JobRunner.Core.DTO;
using JobRunner.Core.Entities.Enums;
using JobRunner.Core.Interfaces.Events.TaskCrud;
using JobRunner.Core.Interfaces.Events.TaskStatus;
using JobRunner.Core.Interfaces.Notification.UI;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace JobRunner.Core.Events.Handlers.UiHandlers
{
    /// <summary>
    /// Обработчик событий задачи для отправки уведомлений в UI
    /// </summary>
    public class UiNotificationHandler :
        IDomainEventHandler<ITaskCreatedEvent>,
        IDomainEventHandler<ITaskUpdatedEvent>,
        IDomainEventHandler<ITaskDeletedEvent>,
        IDomainEventHandler<ITaskStartedEvent>,
        IDomainEventHandler<ITaskCompletedEvent>
    {
        private readonly IUiNotificationService _uiNotification;
        private readonly ILogger<UiNotificationHandler> _logger;

        public UiNotificationHandler(
            IUiNotificationService uiNotification,
            ILogger<UiNotificationHandler> logger)
        {
            _uiNotification = uiNotification;
            _logger = logger;
        }

        public async Task HandleAsync(ITaskCreatedEvent @event, CancellationToken cancellationToken)
        {
            var notification = UiNotification.FromEvent(
                EventType.TaskCreated,
                @event.TaskId,
                @event.TaskName,
                new { @event.CreatedAt, @event.IsEnabled, @event.ScheduleDescription });

            await _uiNotification.NotifyAsync(notification, cancellationToken);
        }

        public async Task HandleAsync(ITaskUpdatedEvent @event, CancellationToken cancellationToken)
        {
            var notification = UiNotification.FromEvent(
                EventType.TaskUpdated,
                @event.TaskId,
                @event.TaskName,
                new { @event.UpdatedAt, @event.ChangedFields });

            await _uiNotification.NotifyAsync(notification, cancellationToken);
        }

        public async Task HandleAsync(ITaskDeletedEvent @event, CancellationToken cancellationToken)
        {
            var notification = UiNotification.FromEvent(
                EventType.TaskDeleted,
                @event.TaskId,
                @event.TaskName);

            await _uiNotification.NotifyAsync(notification, cancellationToken);
        }

        public async Task HandleAsync(ITaskStartedEvent @event, CancellationToken cancellationToken)
        {
            var notification = UiNotification.FromEvent(
                EventType.TaskStarted,
                @event.TaskId,
                @event.TaskName,
                new { @event.StartTime, @event.ProcessId, @event.ExecutionPath });

            await _uiNotification.NotifyAsync(notification, cancellationToken);
        }

        public async Task HandleAsync(ITaskCompletedEvent @event, CancellationToken cancellationToken)
        {
            var eventType = @event.Success ? EventType.TaskCompleted : EventType.TaskFailed;

            var notification = UiNotification.FromEvent(
                eventType,
                @event.TaskId,
                @event.TaskName,
                new { @event.Success, @event.DurationMs, @event.ErrorMessage, @event.CompletionTime });

            await _uiNotification.NotifyAsync(notification, cancellationToken);
        }
    }
}
