using JobRunner.Core.Models.Enums.NotificationEnums;
using System;
using System.Collections.Generic;

namespace JobRunner.Core.Interfaces.Entities.JobTaskSettings.NotifySettings
{
    /// <summary>
    /// Получатель уведомления (ссылка на пользователя)
    /// </summary>
    /// <typeparam name="TUserId">Тип идентификатора пользователя</typeparam>
    public interface INotificationRecipient<TUserId> where TUserId : IEquatable<TUserId>
    {
        /// <summary>
        /// ID получателя (уникальный в рамках задачи)
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// ID пользователя (ссылка на UserModel)
        /// </summary>
        TUserId UserId { get; set; }

        /// <summary>
        /// Имя пользователя (кэш для отображения)
        /// </summary>
        string? UserName { get; set; }

        /// <summary>
        /// Какие способы уведомления использовать для этого пользователя
        /// Если null — используются все доступные способы
        /// </summary>
        List<NotificationType>? Methods { get; set; }

        /// <summary>
        /// Включен ли получатель
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Дополнительные настройки для этого получателя
        /// </summary>
        Dictionary<string, object>? Metadata { get; set; }
    }
}