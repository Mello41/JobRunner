using System.ComponentModel.DataAnnotations;

namespace JobRunner.Core.Entities.Enums
{
    /// <summary>
    /// Уровень важности уведомления
    /// </summary>
    public enum NotificationSeverity
    {
        [Display(Name = "Информационное сообщение (низкая важность)")]
        Info,

        [Display(Name = "Успешное выполнение операции")]
        Success,

        [Display(Name = "Предупреждение (требует внимания, не критично, но может упасть)")]
        Warning,

        [Display(Name = "Критическая ошибка (требует немедленного вмешательства)")]
        Error
    }
}