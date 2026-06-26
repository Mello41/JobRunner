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
        /// id в программе (удобно работать с long/guid и тд)
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// любой id (на всякий)
        /// </summary>
        TUserId UserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string? UserName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Контакты
        /// </summary>
        INotificationRecipientContact Contacts { get; set; }

        /// <summary>
        /// Метаданные
        /// </summary>
        Dictionary<string, object>? Metadata { get; set; }

        #region Методы для работы с контактами
        /// <summary>
        /// Получить данные (контакты) для уведомлений
        /// (по NotificationType)
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        string? GetContact(NotificationType method);

        /// <summary>
        /// получить конкретные типы уведомлений у получателя 
        /// (которые он заполнил)
        /// </summary>
        /// <returns></returns>
        List<NotificationType> GetAvailableMethods();

        /// <summary>
        /// имеет ли пользователь контакт
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        bool HasContact(NotificationType method);

        /// <summary>
        /// имеет ли пользователь контакт (хоть какой либо)
        /// </summary>
        /// <returns></returns>
        bool HasAnyContact();

        #endregion
    }
}