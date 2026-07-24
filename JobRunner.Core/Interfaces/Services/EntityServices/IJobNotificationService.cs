using JobRunner.Core.Interfaces.Entities.JobTaskSettings.NotifySettings;
using JobRunner.Core.Models.Enums.NotificationEnums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JobRunner.Core.Interfaces.Services.EntityServices
{
    /// <summary>
    /// Сервис уведомлений (абстракция) по каждому статусу задачи
    /// </summary>
    /// <typeparam name="TID"></typeparam>
    public interface IJobNotificationService<TID> where TID : IEquatable<TID>
    {
        /// <summary>
        /// Отправить уведомления для указанного статуса задачи
        /// </summary>
        /// <param name="taskId">Идентификатор задачи</param>
        /// <param name="taskName">Имя задачи</param>
        /// <param name="status">Статус задачи</param>
        /// <param name="settings">Настройки уведомлений (содержит получателей и шаблоны)</param>
        /// <param name="isSuccess">Успешность выполнения (для OnCompleted/OnFailed)</param>
        /// <param name="errorMessage">Сообщение об ошибке</param>
        /// <param name="ct">Токен отмены</param>
        Task NotifyForStatusAsync(TID taskId, string taskName, JobNotificationState status,
                                  INotifySettings<TID> settings, bool? isSuccess = null,
                                  string? errorMessage = null, CancellationToken ct = default);

        #region Дополнительные методы

        /// <summary>
        /// Отправить тестовое уведомление для проверки настроек
        /// </summary>
        /// <param name="taskId">Идентификатор задачи</param>
        /// <param name="taskName">Имя задачи</param>
        /// <param name="status">Статус для теста</param>
        /// <param name="settings">Настройки уведомлений</param>
        /// <param name="ct">Токен отмены</param>
        Task SendTestNotificationAsync(TID taskId, string taskName,
                                       JobNotificationState status,
                                       INotifySettings<TID> settings,
                                       CancellationToken ct = default);

        /// <summary>
        /// Проверить, есть ли у задачи активные настройки уведомлений
        /// </summary>
        /// <param name="settings">Настройки уведомлений</param>
        /// <returns>true, если есть хотя бы один включенный статус с получателями</returns>
        bool HasActiveNotifications(INotifySettings<TID>? settings);

        /// <summary>
        /// Получить все активные статусы для задачи
        /// </summary>
        /// <param name="settings">Настройки уведомлений</param>
        /// <returns>Список статусов, для которых включены уведомления</returns>
        List<JobNotificationState> GetActiveStatuses(INotifySettings<TID>? settings);

        /// <summary>
        /// Получить количество получателей для указанного статуса
        /// </summary>
        /// <param name="settings">Настройки уведомлений</param>
        /// <param name="status">Статус задачи</param>
        /// <returns>Количество получателей</returns>
        int GetRecipientsCount(INotifySettings<TID>? settings, 
                               JobNotificationState status);

        /// <summary>
        /// Получить все способы уведомления, доступные для получателей в указанном статусе
        /// </summary>
        /// <param name="settings">Настройки уведомлений</param>
        /// <param name="status">Статус задачи</param>
        /// <returns>Список способов уведомления</returns>
        List<NotificationType> GetAvailableMethodsForStatus(INotifySettings<TID>? settings, 
                                                JobNotificationState status);

        /// <summary>
        /// Проверить, может ли получатель получить уведомление указанным способом
        /// </summary>
        /// <param name="recipient">Получатель</param>
        /// <param name="method">Способ уведомления</param>
        /// <returns>true, если получатель поддерживает указанный способ</returns>
        bool CanNotifyRecipient(INotificationRecipient<TID> recipient, NotificationType method);

        #endregion

        #region Отправка уведомлений
        /// <summary>
        /// Отправить уведомления всем получателям для указанного статуса
        /// (с проверкой всех способов связи)
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="taskName"></param>
        /// <param name="status"></param>
        /// <param name="settings"></param>
        /// <param name="isSuccess"></param>
        /// <param name="errorMessage"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task NotifyAllForStatusAsync(TID taskId, string taskName, JobNotificationState status, 
                                     INotifySettings<TID>? settings, bool? isSuccess = null,
                                     string? errorMessage = null, CancellationToken ct = default);

        /// <summary>
        /// Отправить уведомление конкретному получателю
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="taskName"></param>
        /// <param name="message"></param>
        /// <param name="recipient"></param>
        /// <param name="status"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task SendToRecipientAsync(long taskId, string taskName,
                                  string message, INotificationRecipient<long> recipient,
                                  JobNotificationState status, CancellationToken ct = default);

        /// <summary>
        /// Отправить тестовое Email уведомление
        /// </summary>
        /// <param name="email"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task SendTestEmailAsync(string email, string? subject = null,
                                string? message = null, CancellationToken ct = default);
        #endregion
    }
}