using System;

namespace JobRunner.Core.Entities.ValueObjects
{
    /// <summary>
    /// Интерфейс настроек уведомления задачи
    /// </summary>
    public interface INotifySettings
    {
        /// <summary>
        /// Уведомлять до выполнения
        /// </summary>
        bool NotifyBefore { get; set; }

        /// <summary>
        /// Уведомлять после выполнения
        /// </summary>
        bool NotifyAfter { get; set; }

        /// <summary>
        /// За сколько времени до выполнения уведомить
        /// </summary>
        TimeSpan NotifyBeforeMinutes { get; set; }

        /// <summary>
        /// Текст уведомления
        /// </summary>
        string NotificationMessage { get; set; }
    }
}
