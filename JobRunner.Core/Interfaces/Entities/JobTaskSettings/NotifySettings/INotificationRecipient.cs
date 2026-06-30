using JobRunner.Core.Models.Enums.NotificationEnums;
using System;
using System.Collections.Generic;

namespace JobRunner.Core.Interfaces.Entities.JobTaskSettings.NotifySettings
{
    /// <summary>
    /// Получатель уведомления (ссылка на пользователя)
    /// </summary>
    /// <typeparam name="TId">Тип идентификатора получателя</typeparam>
    public interface INotificationRecipient<TId> where TId : IEquatable<TId>
    {
        /// <summary>
        /// Внутренний идентификатор получателя (первичный ключ)
        /// </summary>
        TId Id { get; set; }

        /// <summary>
        /// Идентификатор пользователя, которому принадлежит получатель
        /// </summary>
        TId UserId { get; set; }

        /// <summary>
        /// Имя пользователя
        /// </summary>
        string? UserName { get; set; }

        /// <summary>
        /// Активен ли получатель
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