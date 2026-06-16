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
        /// <param name="evt">Событие создания задачи</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns></returns>
        public async Task HandleAsync(ITaskCreatedEvent<TId> @evt, CancellationToken cancellationToken)
        {
            var notification = UiNotification.FromEvent(
                EventType.TaskCreated,
                @evt.TaskId?.ToString() ?? string.Empty,
                @evt.TaskName,
                new { @evt.CreatedAt, @evt.IsEnabled, @evt.ScheduleDescription });

            await _uiNotification.NotifyAsync(notification, cancellationToken);
        }

        /// <summary>
        /// Обрабатывает событие обновления задачи и отправляет уведомление в UI
        /// </summary>
        /// <param name="evt">Событие обновления задачи</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns></returns>
        public async Task HandleAsync(ITaskUpdatedEvent<TId> @evt, CancellationToken cancellationToken)
        {
            var notification = UiNotification.FromEvent(
                EventType.TaskUpdated,
                @evt.TaskId?.ToString() ?? string.Empty,
                @evt.TaskName,
                new { @evt.UpdatedAt, @evt.ChangedFields });

            await _uiNotification.NotifyAsync(notification, cancellationToken);
        }

        /// <summary>
        /// Обрабатывает событие удаления задачи и отправляет уведомление в UI
        /// </summary>
        /// <param name="evt">Событие удаления задачи</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns></returns>
        public async Task HandleAsync(ITaskDeletedEvent<TId> @evt, CancellationToken cancellationToken)
        {
            var notification = UiNotification.FromEvent(
                EventType.TaskDeleted,
                @evt.TaskId?.ToString() ?? string.Empty,
                @evt.TaskName);

            await _uiNotification.NotifyAsync(notification, cancellationToken);
        }

        /// <summary>
        /// Обрабатывает событие запуска задачи и отправляет уведомление в UI
        /// </summary>
        /// <param name="evt">Событие запуска задачи</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns></returns>
        public async Task HandleAsync(ITaskStartedEvent<TId> @evt, CancellationToken cancellationToken)
        {
            var notification = UiNotification.FromEvent(
                EventType.TaskStarted,
                @evt.TaskId?.ToString() ?? string.Empty,
                @evt.TaskName,
                new { @evt.StartTime, @evt.ProcessId, @evt.ExecutionPath });

            await _uiNotification.NotifyAsync(notification, cancellationToken);
        }

        /// <summary>
        /// Обрабатывает событие завершения задачи и отправляет уведомление в UI.
        /// Тип уведомления зависит от успешности выполнения: TaskCompleted или TaskFailed
        /// </summary>
        /// <param name="evt">Событие завершения задачи</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns></returns>
        public async Task HandleAsync(ITaskCompletedEvent<TId> @evt, CancellationToken cancellationToken)
        {
            var eventType = @evt.Success ? EventType.TaskCompleted : EventType.TaskFailed;

            var notification = UiNotification.FromEvent(
                eventType,
                @evt.TaskId?.ToString() ?? string.Empty,
                @evt.TaskName,
                new { @evt.Success, @evt.DurationMs, @evt.ErrorMessage, @evt.CompletionTime });

            await _uiNotification.NotifyAsync(notification, cancellationToken);
        }
    }
}
