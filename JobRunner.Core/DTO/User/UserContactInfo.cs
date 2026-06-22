using JobRunner.Core.Models.Enums.NotificationEnums;
using System;
using System.Collections.Generic;

namespace JobRunner.Core.DTO.User
{
    /// <summary>
    /// Контактная информация пользователя для уведомлений
    /// </summary>
    /// <remarks>
    /// Заполняется на сервере на основе данных авторизованного пользователя.
    /// Core не знает, откуда берутся эти данные - это ответственность сервера.
    /// </remarks>
    public class UserContactInfo<TUserId> where TUserId : IEquatable<TUserId>
    {
        /// <summary>
        /// ID пользователя
        /// </summary>
        public TUserId UserId { get; set; } = default!;

        /// <summary>
        /// Имя пользователя (для отображения)
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Email для уведомлений
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Telegram Chat ID
        /// </summary>
        public string? TelegramChatId { get; set; }

        /// <summary>
        /// Webhook URL (для этого пользователя)
        /// </summary>
        public string? WebhookUrl { get; set; }

        /// <summary>
        /// Дополнительные контакты (ключ - тип уведомления, значение - контакт)
        /// </summary>
        public Dictionary<NotificationType, string> AdditionalContacts { get; set; } = new();
    }
}
