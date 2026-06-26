using JobRunner.Core.Events.TaskEvents.TaskStatus;
using JobRunner.Core.Interfaces.Entities.JobTaskSettings.NotifySettings;
using JobRunner.Core.Interfaces.Events.DomainEvent;
using JobRunner.Core.Interfaces.Services.EntityServices;
using JobRunner.Core.Models.Enums.NotificationEnums;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JobRunner.Core.Events.Handlers
{
    /// <summary>
    /// Обработчик событий задачи для отправки уведомлений
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public class SendNotificationHandler<TId> :
                IDomainEventHandler<TaskStartedEvent<TId>>,
                IDomainEventHandler<TaskCompletedEvent<TId>>,
                IDomainEventHandler<TaskFailedEvent<TId>>,
                IDomainEventHandler<TaskStoppedEvent<TId>>,
                IDomainEventHandler<TaskPausedEvent<TId>>,
                IDomainEventHandler<TaskResumedEvent<TId>>,
                IDomainEventHandler<TaskSkippedEvent<TId>>
                where TId : IEquatable<TId>
    {
        private readonly IJobNotificationService<TId> _notificationService;
        private readonly ILogger<SendNotificationHandler<TId>> _logger;

        public SendNotificationHandler(
            IJobNotificationService<TId> notificationService,
            ILogger<SendNotificationHandler<TId>> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        #region Обработчики событий

        public async Task HandleAsync(TaskStartedEvent<TId> @evt, CancellationToken cancellationToken)
        {
            await HandleNotificationForStatus(@evt, JobNotificationState.OnStarted, cancellationToken);
        }

        public async Task HandleAsync(TaskCompletedEvent<TId> @evt, CancellationToken cancellationToken)
        {
            await HandleNotificationForStatus(@evt, JobNotificationState.OnCompleted, cancellationToken, @evt.Success, @evt.ErrorMessage);
        }

        public async Task HandleAsync(TaskFailedEvent<TId> @evt, CancellationToken cancellationToken)
        {
            await HandleNotificationForStatus(@evt, JobNotificationState.OnFailed, cancellationToken, false, @evt.ErrorMessage);
        }

        public async Task HandleAsync(TaskStoppedEvent<TId> @evt, CancellationToken cancellationToken)
        {
            await HandleNotificationForStatus(@evt, JobNotificationState.OnStopped, cancellationToken);
        }

        public async Task HandleAsync(TaskPausedEvent<TId> @evt, CancellationToken cancellationToken)
        {
            await HandleNotificationForStatus(@evt, JobNotificationState.OnPaused, cancellationToken);
        }

        public async Task HandleAsync(TaskResumedEvent<TId> @evt, CancellationToken cancellationToken)
        {
            await HandleNotificationForStatus(@evt, JobNotificationState.OnResumed, cancellationToken);
        }

        public async Task HandleAsync(TaskSkippedEvent<TId> @evt, CancellationToken cancellationToken)
        {
            await HandleNotificationForStatus(@evt, JobNotificationState.OnSkipped, cancellationToken);
        }

        #endregion

        #region Основной метод обработки

        /// <summary>
        /// Обработка уведомления для конкретного статуса
        /// </summary>
        private async Task HandleNotificationForStatus(
            object evt,
            JobNotificationState status,
            CancellationToken cancellationToken,
            bool? isSuccess = null,
            string? errorMessage = null)
        {
            // Извлекаем общие данные из события через рефлексию или паттерн
            // Так как все события имеют TaskId, TaskName, NotifySettings
            var taskId = GetTaskId(evt);
            var taskName = GetTaskName(evt);
            var settings = GetNotifySettings(evt);

            try
            {
                // Проверяем, включены ли уведомления глобально
                if (settings?.EnableNotifications != true)
                {
                    _logger.LogDebug("Notifications disabled globally for task {TaskId}", taskId);
                    return;
                }

                // Получаем настройки для конкретного статуса
                var statusSettings = settings.GetStatusSettings(status);

                // Проверяем, включены ли уведомления для этого статуса
                if (statusSettings == null || !statusSettings.IsEnabled)
                {
                    _logger.LogDebug("Notifications for status {Status} disabled for task {TaskId}", status, taskId);
                    return;
                }

                // Проверяем, есть ли получатели
                if (statusSettings.Recipients == null || statusSettings.Recipients.Count == 0)
                {
                    _logger.LogDebug("No recipients for status {Status} in task {TaskId}", status, taskId);
                    return;
                }

                _logger.LogInformation("Sending notification for task {TaskName} (Id: {TaskId}) with status {Status}", taskName, taskId, status);

                // Формируем сообщение используя метод из настроек
                var message = FormatMessageFromSettings(statusSettings, taskName, status, isSuccess, errorMessage);

                // Отправляем уведомления — используем сервис, который уже знает, как работать с настройками
                await _notificationService.NotifyForStatusAsync(
                    taskId,
                    taskName,
                    status,
                    settings,
                    isSuccess,
                    errorMessage,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send notification for task {TaskId} with status {Status}", taskId, status);
            }
        }

        #endregion

        #region Вспомогательные методы

        /// <summary>
        /// Форматирование сообщения с использованием настроек
        /// </summary>
        private string FormatMessageFromSettings(
            IJobStatusNotificationSettings settings,
            string taskName,
            JobNotificationState status,
            bool? isSuccess,
            string? errorMessage)
        {
            var template = settings.MessageTemplate;

            if (string.IsNullOrEmpty(template))
            {
                return GetDefaultMessage(taskName, status, isSuccess, errorMessage);
            }

            var message = template.Replace("{TaskName}", taskName);

            var statusText = GetStatusText(status);
            message = message.Replace("{Status}", statusText);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                message = message.Replace("{ErrorMessage}", errorMessage);
            }

            if (settings.DelayBefore.HasValue)
            {
                message = message.Replace("{Delay}", settings.DelayBefore.Value.TotalMinutes.ToString());
            }

            return message;
        }

        private string GetStatusText(JobNotificationState status)
        {
            return status switch
            {
                JobNotificationState.OnCreated => "создана",
                JobNotificationState.OnStarted => "запущена",
                JobNotificationState.OnCompleted => "успешно завершена",
                JobNotificationState.OnFailed => "завершена с ошибкой",
                JobNotificationState.OnStopped => "остановлена",
                JobNotificationState.OnPaused => "приостановлена",
                JobNotificationState.OnResumed => "возобновлена",
                JobNotificationState.OnSkipped => "пропущена",
                _ => status.ToString()
            };
        }

        private string GetDefaultMessage(string taskName, JobNotificationState status, bool? isSuccess, string? errorMessage)
        {
            return status switch
            {
                JobNotificationState.OnStarted => $"Задача '{taskName}' запущена",
                JobNotificationState.OnCompleted => $"Задача '{taskName}' успешно выполнена",
                JobNotificationState.OnFailed => $"Ошибка в задаче '{taskName}': {errorMessage ?? "неизвестная ошибка"}",
                JobNotificationState.OnStopped => $"Задача '{taskName}' остановлена",
                JobNotificationState.OnPaused => $"Задача '{taskName}' приостановлена",
                JobNotificationState.OnResumed => $"Задача '{taskName}' возобновлена",
                JobNotificationState.OnSkipped => $"Задача '{taskName}' пропущена",
                JobNotificationState.OnCreated => $"Задача '{taskName}' создана",
                _ => $"Уведомление по задаче '{taskName}'"
            };
        }

        #endregion

        #region Извлечение данных из событий (через рефлексию для обобщенности)

        private TId GetTaskId(object evt)
        {
            var prop = evt.GetType().GetProperty("TaskId");
            return prop != null ? (TId)prop.GetValue(evt)! : default!;
        }

        private string GetTaskName(object evt)
        {
            var prop = evt.GetType().GetProperty("TaskName");
            return prop != null ? (string)prop.GetValue(evt)! : "Unknown";
        }

        private INotifySettings<TId>? GetNotifySettings(object evt)
        {
            var prop = evt.GetType().GetProperty("NotifySettings");
            return prop != null ? (INotifySettings<TId>?)prop.GetValue(evt) : null;
        }

        #endregion
    }
}