using JobRunner.Core.DTO;
using JobRunner.Core.Entities.Enums;
using JobRunner.Core.Interfaces.Events.DomainEvent;
using JobRunner.Core.Interfaces.Events.TaskCrud;
using JobRunner.Core.Interfaces.Events.TaskStatus;
using JobRunner.Core.Interfaces.Notification.UI;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JobRunner.Core.Events.Handlers.UiHandlers
{
    /// <summary>
    /// Обработчик событий задачи для отправки уведомлений в UI
    /// </summary>
    public class UiNotificationHandler<TId> :
                    IDomainEventHandler<ITaskCreatedEvent<TId>>,
                    IDomainEventHandler<ITaskUpdatedEvent<TId>>,
                    IDomainEventHandler<ITaskDeletedEvent<TId>>,
                    IDomainEventHandler<ITaskStartedEvent<TId>>,
                    IDomainEventHandler<ITaskCompletedEvent<TId>>
                    where TId : IEquatable<TId>
    {
        private readonly IUiNotificationService _uiNotification;
        private readonly ILogger<UiNotificationHandler<TId>> _logger;

        public UiNotificationHandler(
            IUiNotificationService uiNotification,
            ILogger<UiNotificationHandler<TId>> logger)
        {
            _uiNotification = uiNotification;
            _logger = logger;
        }

        /// <summary>
        /// Обрабатывает событие создания задачи и отправляет уведомление в UI
        /// </summary>
        /// <param name="event">Событие создания задачи</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns></returns>
        public async Task HandleAsync(ITaskCreatedEvent<TId> @event, CancellationToken cancellationToken)
        {
            var notification = UiNotification.FromEvent(
                EventType.TaskCreated,
                @event.TaskId?.ToString() ?? string.Empty,
                @event.TaskName,
                new { @event.CreatedAt, @event.IsEnabled, @event.ScheduleDescription });

            await _uiNotification.NotifyAsync(notification, cancellationToken);
        }

        /// <summary>
        /// Обрабатывает событие обновления задачи и отправляет уведомление в UI
        /// </summary>
        /// <param name="event">Событие обновления задачи</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns></returns>
        public async Task HandleAsync(ITaskUpdatedEvent<TId> @event, CancellationToken cancellationToken)
        {
            var notification = UiNotification.FromEvent(
                EventType.TaskUpdated,
                @event.TaskId?.ToString() ?? string.Empty,
                @event.TaskName,
                new { @event.UpdatedAt, @event.ChangedFields });

            await _uiNotification.NotifyAsync(notification, cancellationToken);
        }

        /// <summary>
        /// Обрабатывает событие удаления задачи и отправляет уведомление в UI
        /// </summary>
        /// <param name="event">Событие удаления задачи</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns></returns>
        public async Task HandleAsync(ITaskDeletedEvent<TId> @event, CancellationToken cancellationToken)
        {
            var notification = UiNotification.FromEvent(
                EventType.TaskDeleted,
                @event.TaskId?.ToString() ?? string.Empty,
                @event.TaskName);

            await _uiNotification.NotifyAsync(notification, cancellationToken);
        }

        /// <summary>
        /// Обрабатывает событие запуска задачи и отправляет уведомление в UI
        /// </summary>
        /// <param name="event">Событие запуска задачи</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns></returns>
        public async Task HandleAsync(ITaskStartedEvent<TId> @event, CancellationToken cancellationToken)
        {
            var notification = UiNotification.FromEvent(
                EventType.TaskStarted,
                @event.TaskId?.ToString() ?? string.Empty,
                @event.TaskName,
                new { @event.StartTime, @event.ProcessId, @event.ExecutionPath });

            await _uiNotification.NotifyAsync(notification, cancellationToken);
        }

        /// <summary>
        /// Обрабатывает событие завершения задачи и отправляет уведомление в UI.
        /// Тип уведомления зависит от успешности выполнения: TaskCompleted или TaskFailed
        /// </summary>
        /// <param name="event">Событие завершения задачи</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns></returns>
        public async Task HandleAsync(ITaskCompletedEvent<TId> @event, CancellationToken cancellationToken)
        {
            var eventType = @event.Success ? EventType.TaskCompleted : EventType.TaskFailed;

            var notification = UiNotification.FromEvent(
                eventType,
                @event.TaskId?.ToString() ?? string.Empty,
                @event.TaskName,
                new { @event.Success, @event.DurationMs, @event.ErrorMessage, @event.CompletionTime });

            await _uiNotification.NotifyAsync(notification, cancellationToken);
        }
    }
}
