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
    /// Этот обработчик только маршрутизирует уведомления.
    /// Реальная отправка делегируется INotificationSender.
    /// </summary>
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
                // Здесь нужен полный объект задачи, а не только событие
                // Проблема: ITaskStartedEvent не содержит INotifySettings
                // 
                // Варианты решения:
                // 1. Расширить ITaskStartedEvent, добавив TaskId и загружать задачу из БД
                // 2. Передавать IJobTask в событии
                // 3. Использовать отдельный механизм для уведомлений до выполнения

                _logger.LogInformation(
                    "Task {TaskId} started, checking if before-notification needed",
                    @event.TaskId);

                // TODO: Получить задачу из БД через ITaskService
                // var task = await _taskService.GetByIdAsync(@event.TaskId, cancellationToken);
                // if (task?.NotifySettings.NotifyBefore == true)
                // {
                //     await _notificationService.NotifyBeforeAsync(task, cancellationToken);
                // }
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
                _logger.LogInformation(
                    "Task {TaskId} completed (Success: {Success}), checking if after-notification needed",
                    @event.TaskId, @event.Success);

                // TODO: Получить задачу из БД через ITaskService
                // var task = await _taskService.GetByIdAsync(@event.TaskId, cancellationToken);
                // if (task?.NotifySettings.NotifyAfter == true)
                // {
                //     await _notificationService.NotifyAsync(task, @event.Success, @event.ErrorMessage, cancellationToken);
                // }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send after-notification for task {TaskId}", @event.TaskId);
            }
        }
    }
}