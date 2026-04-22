using JobRunner.Core.Entities;
using System;

namespace JobRunner.Core.DefaultImplementations
{
    /// <summary>
    /// Настройки уведомления задачи JobTask
    /// </summary>
    public class NotifySettings : INotifySettings
    {
        /// <summary>
        /// Уведомлять до выполнения (за сколько минут)
        /// </summary>
        public bool NotifyBefore { get; set; }

        /// <summary>
        /// Уведомлять после выполнения
        /// </summary>
        public bool NotifyAfter { get; set; }

        /// <summary>
        /// За сколько времени до выполнения уведомить (в минутах)
        /// </summary>
        public TimeSpan NotifyBeforeMinutes { get; set; }

        /// <summary>
        /// Текст уведомления
        /// </summary>
        public string NotificationMessage { get; set; } = string.Empty;

    }
}
