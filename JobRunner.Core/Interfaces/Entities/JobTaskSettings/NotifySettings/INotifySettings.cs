using JobRunner.Core.Models.Enums.NotificationEnums;
using System;
using System.Collections.Generic;

namespace JobRunner.Core.Interfaces.Entities.JobTaskSettings.NotifySettings
{
    /// <summary>
    /// Интерфейс настроек уведомления задачи
    /// </summary>
    /// <typeparam name="TId">Тип идентификатора задачи</typeparam>
    public interface INotifySettings<TId> where TId : IEquatable<TId>
    {
        /// <summary>
        /// Уникальный идентификатор задачи
        /// </summary>
        TId JobTaskId { get; set; }

        /// <summary>
        /// Разрешение на уведомления (глобальный выключатель)
        /// </summary>
        bool EnableNotifications { get; set; }

        /// <summary>
        /// Настройки уведомлений по статусам задачи.
        /// Ключ — состояние задачи (JobNotificationState),
        /// Значение — настройки для этого состояния
        /// </summary>
        Dictionary<JobNotificationState, 
            IJobStatusNotificationSettings<TId>> StatusSettings { get; set; }

        #region Методы

        /// <summary>
        /// Получить или создать настройки для указанного статуса
        /// </summary>
        /// <param name="state">Состояние задачи</param>
        /// <returns>Настройки уведомлений для данного статуса</returns>
        IJobStatusNotificationSettings<TId> GetOrCreateStatusSettings(JobNotificationState state);

        /// <summary>
        /// Получить настройки для указанного статуса
        /// </summary>
        /// <param name="state">Состояние задачи</param>
        /// <returns>Настройки уведомлений или null, если статус не найден</returns>
        IJobStatusNotificationSettings<TId>? GetStatusSettings(JobNotificationState state);

        /// <summary>
        /// Добавить получателя к указанному статусу
        /// </summary>
        /// <param name="state">Состояние задачи</param>
        /// <param name="recipient">Получатель уведомлений</param>
        void AddRecipientToStatus(JobNotificationState state, INotificationRecipient<TId> recipient);

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
        List<INotificationRecipient<TId>> GetRecipientsForStatus(JobNotificationState state);

        /// <summary>
        /// Получить получателей для указанного статуса по способу уведомления
        /// </summary>
        /// <param name="state">Состояние задачи</param>
        /// <param name="method">Способ уведомления</param>
        /// <returns>Список получателей, у которых есть указанный способ связи</returns>
        List<INotificationRecipient<TId>> GetRecipientsForStatusAndMethod(
                                                JobNotificationState state,
                                                NotificationType method);

        #endregion
    }
}