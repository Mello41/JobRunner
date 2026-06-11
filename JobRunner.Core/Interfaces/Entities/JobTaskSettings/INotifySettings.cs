using JobRunner.Core.Entities.Enums;
using System;
using System.Collections.Generic;

namespace JobRunner.Core.Interfaces.Entities.JobTaskSettings
{
    /// <summary>
    /// Интерфейс настроек уведомления задачи
    /// </summary>
    public interface INotifySettings
    {
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

        #region уведомления сразу
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
