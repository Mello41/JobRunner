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
        /// Разрешение на уведомления
        /// </summary>
        bool EnableNotifications { get; set; }

        #region уведомления до
        /// <summary>
        /// Уведомлять до выполнения
        /// </summary>
        bool NotifyBefore { get; set; }

        /// <summary>
        /// За сколько времени до выполнения уведомить
        /// </summary>
        TimeSpan NotifyBeforeTime { get; set; }
        #endregion

        #region уведомления при старте
        /// <summary>
        /// Уведомлять сразу после старта
        /// </summary>
        bool NotifyOnStarted { get; set; }

        /// <summary>
        /// За сколько времени до выполнения уведомить
        /// </summary>
        TimeSpan NotifyOnStartedTime { get; set; }
        #endregion

        #region уведомления после
        /// <summary>
        /// Уведомлять после выполнения
        /// </summary>
        bool NotifyAfter { get; set; }

        /// <summary>
        /// За сколько времени до выполнения уведомить
        /// </summary>
        TimeSpan NotifyAfterTime { get; set; }
        #endregion

        /// <summary>
        /// Текст уведомления
        /// </summary>
        string NotificationMessage { get; set; }

        /// <summary>
        /// Способы уведомления (можно несколько)
        /// </summary>
        List<NotificationType> NotificationMethods { get; set; }

        #region Получатели (пользователи)
        /// <summary>
        /// Список получателей (пользователей)
        /// </summary>
        List<INotificationRecipient<TUserId>> Recipients { get; set; }

        /// <summary>
        /// Добавить получателя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="methods"></param>
        void AddRecipient(TUserId userId, List<NotificationType>? methods = null);

        /// <summary>
        /// Удалить получателя
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        bool RemoveRecipient(TUserId userId);

        /// <summary>
        /// Получить всех получателей для конкретного способа уведомления
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        List<TUserId> GetRecipientsForMethod(NotificationType method);

        /// <summary>
        /// Получить контактные данные получателя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        string? GetContactForRecipient(TUserId userId, NotificationType method);
        #endregion

        /// <summary>
        /// Форматирует сообщение уведомления с подстановкой параметров
        /// </summary>
        /// <param name="taskName">Имя задачи</param>
        /// <param name="isSuccess">Успех или ошибка</param>
        /// <param name="errorMessage">Сообщение об ошибке (опционально)</param>
        /// <returns>Отформатированное сообщение</returns>
        string FormatMessage(string taskName, bool isSuccess, string? errorMessage = null);
    }
}