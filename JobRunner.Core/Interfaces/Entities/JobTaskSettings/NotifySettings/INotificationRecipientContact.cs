namespace JobRunner.Core.Interfaces.Entities.JobTaskSettings.NotifySettings
{
    /// <summary>
    /// Контактные данные получателя уведомления
    /// </summary>
    public interface INotificationRecipientContact
    {
        /// <summary>
        /// Телелефон
        /// </summary>
        string? Phone { get; set; }

        /// <summary>
        /// Почта
        /// </summary>
        string? Email { get; set; }

        /// <summary>
        /// Телеграмм username/id (по ситуации)
        /// </summary>
        string? TelegramId { get; set; }

        /// <summary>
        /// HTTP веб хук (пригодится)
        /// </summary>
        string? Webhook { get; set; }
    }
}