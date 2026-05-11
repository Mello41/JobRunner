using System;
using System.Threading;
using System.Threading.Tasks;
using JobRunner.Core.Interfaces.Events.TaskStatus;
using JobRunner.Core.Interfaces.Notification;
using Microsoft.Extensions.Logging;

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
    public class SendNotificationHandler :
        IDomainEventHandler<ITaskStartedEvent>,
        IDomainEventHandler<ITaskCompletedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<SendNotificationHandler> _logger;

        public SendNotificationHandler(
            INotificationService notificationService,
            ILogger<SendNotificationHandler> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        /// <summary>
        /// Обработка события "Задача запущена" → уведомление ДО выполнения
        /// </summary>
        public async Task HandleAsync(ITaskStartedEvent @event, CancellationToken cancellationToken)
        {
            try
            {
                if (@event.NotifySettings?.NotifyBefore != true)
                {
                    _logger.LogDebug(
                        "Before-notification skipped for task {TaskId} (NotifyBefore=false or null)",
                        @event.TaskId);
                    return;
                }

                _logger.LogInformation(
                    "Sending before-notification for task {TaskName} (Id: {TaskId})",
                    @event.TaskName, @event.TaskId);

                await _notificationService.NotifyBeforeAsync(
                    @event.TaskId,
                    @event.TaskName,
                    @event.NotifySettings,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send before-notification for task {TaskId}", @event.TaskId);
            }
        }

        /// <summary>
        /// Обработка события "Задача завершена" → уведомление ПОСЛЕ выполнения
        /// </summary>
        public async Task HandleAsync(ITaskCompletedEvent @event, CancellationToken cancellationToken)
        {
            try
            {
                if (@event.NotifySettings?.NotifyAfter != true)
                {
                    _logger.LogDebug(
                        "After-notification skipped for task {TaskId} (NotifyAfter=false or null)",
                        @event.TaskId);
                    return;
                }

                var status = @event.Success ? "successfully" : "with error";
                _logger.LogInformation(
                    "Sending after-notification for task {TaskName} (Id: {TaskId}) completed {Status}",
                    @event.TaskName, @event.TaskId, status);

                await _notificationService.NotifyAsync(
                    @event.TaskId,
                    @event.TaskName,
                    @event.Success,
                    @event.ErrorMessage,
                    @event.NotifySettings,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send after-notification for task {TaskId}", @event.TaskId);
            }
        }
    }
}