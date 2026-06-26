using JobRunner.Core.Models.Enums.NotificationEnums;
using System;
using System.Collections.Generic;

namespace JobRunner.Core.Interfaces.Entities.JobTaskSettings.NotifySettings
{
    /// <summary>
    /// Интерфейс настроек уведомления задачи
    /// </summary>
    /// <typeparam name="TUserId"></typeparam>
    public interface INotifySettings<TUserId> where TUserId : IEquatable<TUserId>
    {
        /// <summary>
        /// Разрешение на уведомления (глобальный выключатель)
        /// </summary>
        bool EnableNotifications { get; set; }

        /// <summary>
        /// Настройки уведомлений по статусам задачи.
        /// Ключ — состояние задачи (JobNotificationState),
        /// Значение — настройки для этого состояния
        /// </summary>
        Dictionary<JobNotificationState, IJobStatusNotificationSettings> StatusSettings { get; set; }

        #region Методы
        /// <summary>
        /// Получить или создать настройки для указанного статуса
        /// </summary>
        /// <param name="state">Состояние задачи</param>
        /// <returns>Настройки уведомлений для данного статуса</returns>
        IJobStatusNotificationSettings GetOrCreateStatusSettings(JobNotificationState state);

        /// <summary>
        /// Получить настройки для указанного статуса
        /// </summary>
        /// <param name="state">Состояние задачи</param>
        /// <returns>Настройки уведомлений или null, если статус не найден</returns>
        IJobStatusNotificationSettings? GetStatusSettings(JobNotificationState state);

        /// <summary>
        /// Добавить получателя к указанному статусу
        /// </summary>
        /// <param name="state">Состояние задачи</param>
        /// <param name="recipient">Получатель уведомлений</param>
        void AddRecipientToStatus(JobNotificationState state, INotificationRecipient<long> recipient);

        /// <summary>
        /// Удалить получателя из указанного статуса
        /// </summary>
        /// <param name="state">Состояние задачи</param>
        /// <param name="recipientId">Идентификатор получателя</param>
        /// <returns>true, если получатель был найден и удален</returns>
        bool RemoveRecipientFromStatus(JobNotificationState state, string recipientId);

        /// <summary>
        /// Получить всех получателей для указанного статуса
        /// </summary>
        /// <param name="state">Состояние задачи</param>
        /// <returns>Список получателей для данного статуса</returns>
        List<INotificationRecipient<long>> GetRecipientsForStatus(JobNotificationState state);

        /// <summary>
        /// Получить получателей для указанного статуса по способу уведомления
        /// </summary>
        /// <param name="state">Состояние задачи</param>
        /// <param name="method">Способ уведомления</param>
        /// <returns>Список получателей, у которых есть указанный способ связи</returns>
        List<INotificationRecipient<long>> GetRecipientsForStatusAndMethod(JobNotificationState state, NotificationType method);
        #endregion
    }
}