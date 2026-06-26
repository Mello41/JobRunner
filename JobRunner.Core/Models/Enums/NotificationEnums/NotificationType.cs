using System.ComponentModel.DataAnnotations;

namespace JobRunner.Core.Models.Enums.NotificationEnums
{
    /// <summary>
    /// Тип оповещения (внутри программы)
    /// </summary>
    public enum NotificationType
    {
        #region System
        [Display(Name = "Всплывающее окно (Desktop)")]
        Popup,

        [Display(Name = "Звук (Desktop)")]
        Sound,

        [Display(Name = "Файловый лог (сервер)")]
        LogFile,

        [Display(Name = "Sentry, OpenTelemetry")]
        Monitoring,

        [Display(Name = "Windows Event Log")]
        EventLog,
        #endregion

        #region userData
        [Display(Name = "Email (SMTP)")]
        Email,

        [Display(Name = "HTTP-вызов на указанный URL")]
        Webhook,

        [Display(Name = "Telegram уведомление (сообщение)")]
        Telegram,

        [Display(Name = "СМС")]
        SMS
        #endregion
    }
}