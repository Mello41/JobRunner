using JobRunner.Core.Events.TaskEvents.TaskStatus;
using JobRunner.Core.Interfaces.Events.DomainEvent;
using JobRunner.Core.Interfaces.Notification;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JobRunner.Core.Events.Handlers
{
    /// <summary>
    /// Обработчик событий задачи для отправки уведомлений
    /// (Email, Telegram, Webhook, Popup и т.д.)
    /// Этот обработчик только маршрутизирует уведомления на основе настроек из события.
    /// Реальная отправка делегируется INotificationService.
    /// </summary>
    /// <remarks>
    /// Настройки уведомлений приходят прямо в событии (через INotifiableEvent),
    /// поэтому не нужно загружать задачу из БД.
    /// </remarks>
    public class SendNotificationHandler<TId> :
        IDomainEventHandler<TaskStartedEvent<TId>>,
        IDomainEventHandler<TaskCompletedEvent<TId>>
        where TId : IEquatable<TId>
    {
        private readonly INotificationService<TId> _notificationService;
        private readonly ILogger<SendNotificationHandler<TId>> _logger;

        public SendNotificationHandler(
            INotificationService<TId> notificationService,
            ILogger<SendNotificationHandler<TId>> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        /// <summary>
        /// Обработка события "Задача запущена" → уведомление ДО выполнения
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task HandleAsync(TaskStartedEvent<TId> @evt, CancellationToken cancellationToken)
        {
            try
            {
                if (@evt.NotifySettings?.NotifyBefore != true)
                {
                    _logger.LogDebug(
                        "Before-notification skipped for task {TaskId} (NotifyBefore=false or null)",
                        @evt.TaskId);
                    return;
                }

                _logger.LogInformation(
                    "Sending before-notification for task {TaskName} (Id: {TaskId})",
                    @evt.TaskName, @evt.TaskId);

                await _notificationService.NotifyBeforeAsync(
                    @evt.TaskId,
                    @evt.TaskName,
                    @evt.NotifySettings,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send " +
                    "before-notification for task " +
                    "{TaskId}", @evt.TaskId);
            }
        }

        /// <summary>
        /// Обработка события "Задача завершена" → уведомление ПОСЛЕ выполнения
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task HandleAsync(TaskCompletedEvent<TId> @evt, CancellationToken cancellationToken)
        {
            try
            {
                if (@evt.NotifySettings?.NotifyAfter != true)
                {
                    _logger.LogDebug(
                        "After-notification skipped for task {TaskId} (NotifyAfter=false or null)",
                        @evt.TaskId);
                    return;
                }

                var status = @evt.Success ? "successfully" : "with error";
                _logger.LogInformation(
                    "Sending after-notification for task {TaskName} (Id: {TaskId}) completed {Status}",
                    @evt.TaskName, @evt.TaskId, status);

                var formattedMessage = @evt.NotifySettings?.FormatMessage(
                    @evt.TaskName,
                    @evt.Success,
                    @evt.ErrorMessage) ?? GetDefaultMessage(@evt.TaskName, @evt.Success, @evt.ErrorMessage);

                await _notificationService.NotifyAsync(
                    @evt.TaskId,
                    @evt.TaskName,
                    @evt.Success,
                    formattedMessage,
                    @evt.NotifySettings,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send " +
                    "after-notification for task " +
                    "{TaskId}", @evt.TaskId);
            }
        }

        /// <summary>
        /// Получить сообщение по умолчанию
        /// </summary>
        /// <param name="taskName"></param>
        /// <param name="success"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        private static string GetDefaultMessage(string taskName, bool success, string? errorMessage)
        {
            return success
                ? $"Task '{taskName}' completed successfully"
                : $"Task '{taskName}' failed: {errorMessage}";
        }
    }
}