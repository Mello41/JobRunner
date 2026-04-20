namespace JobRunner.Core.Notify
{
    /// <summary>
    /// Настройки уведомления задачи JobTask
    /// </summary>
    public class NotifySettings
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
        public int NotifyBeforeMinutes { get; set; }

        /// <summary>
        /// Текст уведомления
        /// </summary>
        public string NotificationMessage { get; set; } = string.Empty;

        /// <summary>
        /// Способ уведомления
        /// </summary>
        public NotificationType NotifyType { get; set; } 
            = NotificationType.Popup;
    }
}
