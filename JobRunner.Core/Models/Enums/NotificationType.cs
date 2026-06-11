using System.ComponentModel.DataAnnotations;

namespace JobRunner.Core.Entities.Enums
{
    /// <summary>
    /// Тип оповещения (внутри программы)
    /// </summary>
    public enum NotificationType
    {
        [Display(Name = "Всплывающее окно (Desktop)")]
        Popup,

        [Display(Name = "Звук (Desktop)")]
        Sound,

        [Display(Name = "Email (SMTP)")]
        Email,

        [Display(Name = "Файловый лог (сервер)")]
        LogFile,

        [Display(Name = "Windows Event Log")]
        EventLog,

        [Display(Name = "HTTP-вызов на указанный URL")]
        Webhook,

        [Display(Name = "Sentry, OpenTelemetry")]
        Monitoring,

        [Display(Name = "Telegram уведомление (сообщение)")]
        Telegram
    }
}
