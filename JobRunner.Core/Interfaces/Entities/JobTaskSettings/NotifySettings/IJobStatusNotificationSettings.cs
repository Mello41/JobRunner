using JobRunner.Core.Models.Enums.NotificationEnums;
using System;
using System.Collections.Generic;

namespace JobRunner.Core.Interfaces.Entities.JobTaskSettings.NotifySettings
{
    /// <summary>
    /// Настройки уведомлений для конкретного состояния задачи
    /// </summary>
    public interface IJobStatusNotificationSettings
    {
        /// <summary>
        /// Состояние задачи, к которому относятся данные 
        /// настройки (чтобы не потеряться)
        /// </summary>
        JobNotificationState State { get; }

        /// <summary>
        /// Включены ли уведомления для данного состояния
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Уровень важности уведомления
        /// </summary>
        NotificationSeverity Severity { get; set; }

        /// <summary>
        /// Задержка перед отправкой уведомления (только для Before-событий).
        /// Для On-событий значение должно быть null
        /// </summary>
        TimeSpan? DelayBefore { get; set; }

        /// <summary>
        /// Шаблон сообщения уведомления.
        /// Поддерживает подстановку: {TaskName}, {Status}, {ErrorMessage}, {Delay}
        /// </summary>
        string MessageTemplate { get; set; }

        /// <summary>
        /// Список получателей для данного состояния
        /// </summary>
        List<INotificationRecipient<long>> Recipients { get; set; }

        #region Методы

        /// <summary>
        /// Добавить получателя к данному состоянию
        /// </summary>
        /// <param name="recipient">Получатель уведомления</param>
        void AddRecipient(INotificationRecipient<long> recipient);

        /// <summary>
        /// Удалить получателя по идентификатору
        /// </summary>
        /// <param name="recipientId">Идентификатор получателя</param>
        /// <returns>true, если получатель был найден и удален</returns>
        bool RemoveRecipient(string recipientId);

        /// <summary>
        /// Получить список получателей для данного состояния по способу уведомления
        /// </summary>
        /// <param name="method">Способ уведомления (Email, Telegram, Webhook)</param>
        /// <returns>Список получателей, у которых есть указанный способ связи</returns>
        List<INotificationRecipient<long>> GetRecipientsByMethod(NotificationType method);

        /// <summary>
        /// Очистить всех получателей для данного состояния
        /// </summary>
        void ClearRecipients();
        #endregion
    }
}