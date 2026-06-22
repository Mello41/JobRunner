using System.ComponentModel.DataAnnotations;

namespace JobRunner.Core.Models.Enums.RetryStrategies
{
    /// <summary>
    /// Стратегия обработки ошибок в группе
    /// </summary>
    public enum GroupFailureStrategy
    {
        [Display(Name = "Остановить группу при первой ошибке")]
        StopOnFirstFailure,

        [Display(Name = "Продолжить выполнение, но отметить группу как неуспешную")]
        ContinueButMarkFailed,

        [Display(Name = "Продолжить выполнение, игнорируя ошибки")]
        ContinueAndIgnore,

        [Display(Name = "Повторить неудачные задачи в конце группы")]
        RetryFailedAtEnd
    }
}