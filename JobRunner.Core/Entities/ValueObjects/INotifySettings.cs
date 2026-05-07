using JobRunner.Core.Entities.Enums;
using System;
using System.Collections.Generic;

namespace JobRunner.Core.Entities.ValueObjects
{
    /// <summary>
    /// Интерфейс настроек уведомления задачи
    /// </summary>
    public interface INotifySettings
    {
        /// <summary>
        /// Уведомлять до выполнения
        /// </summary>
        bool NotifyBefore { get; set; }

        /// <summary>
        /// Уведомлять после выполнения
        /// </summary>
        bool NotifyAfter { get; set; }

        /// <summary>
        /// За сколько времени до выполнения уведомить
        /// </summary>
        TimeSpan NotifyBeforeMinutes { get; set; }

        /// <summary>
        /// Текст уведомления
        /// </summary>
        string NotificationMessage { get; set; }

        /// <summary>
        /// Способы уведомления (можно несколько)
        /// </summary>
        List<NotificationType> NotificationMethods { get; set; }

        /// <summary>
        /// Email для уведомлений (если выбран Email)
        /// </summary>
        string? NotificationEmail { get; set; }

        /// <summary>
        /// Telegram chat ID (если выбран Telegram)
        /// </summary>
        string? TelegramChatId { get; set; }

        /// <summary>
        /// Webhook URL (если выбран Webhook)
        /// </summary>
        string? WebhookUrl { get; set; }
    }
}
