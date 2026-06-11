using JobRunner.Core.Entities.Enums;
using JobRunner.Core.Interfaces.Entities.JobTaskSettings;
using System;
using System.Collections.Generic;

namespace JobRunner.Core.DefaultImplementations
{
    /// <summary>
    /// Настройки уведомления задачи JobTask
    /// </summary>
    public class NotifySettingsExample //: INotifySettings
    {
        /// <summary>
        /// Уведомлять до выполнения (за сколько минут)
        /// </summary>
        public bool NotifyBefore { get; set; }

        /// <summary>
        /// Уведомлять после выполнения
        /// </summary>
        public bool NotifyAfter { get; set; }

        /// <summary>
        /// За сколько времени до выполнения уведомить (в минутах)
        /// </summary>
        public TimeSpan NotifyBeforeMinutes { get; set; }

        /// <summary>
        /// Текст уведомления
        /// </summary>
        public string NotificationMessage { get; set; } = string.Empty;

        public List<NotificationType> NotificationMethods { get; set; } = new();
        public string? NotificationEmail { get; set; } 
        public string? TelegramChatId { get; set; } 
        public string? WebhookUrl { get; set; }

        public string FormatMessage(string taskName, bool isSuccess, string? errorMessage = null)
        {
            if (!string.IsNullOrWhiteSpace(NotificationMessage))
            {
                var message = NotificationMessage
                    .Replace("{TaskName}", taskName)
                    .Replace("{Status}", isSuccess ? "успешно" : "с ошибкой");

                if (!string.IsNullOrEmpty(errorMessage))
                    message = message.Replace("{ErrorMessage}", errorMessage);

                return message;
            }

            return isSuccess
                ? $"Задача '{taskName}' успешно выполнена"
                : $"Задача '{taskName}' завершилась с ошибкой: {errorMessage}";
        }
    }
}
