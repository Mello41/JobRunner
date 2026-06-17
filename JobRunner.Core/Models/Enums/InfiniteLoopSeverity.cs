using System.ComponentModel.DataAnnotations;

namespace JobRunner.Core.Models.Enums
{
    /// <summary>
    /// уровень серьезности подозрения на 
    /// бесконечный цикл (Infinite Loop) в выполнении задачи
    /// </summary>
    public enum InfiniteLoopSeverity
    {
        [Display(Name = "Предупреждение")]
        Warning,

        [Display(Name = "Критический")]
        Critical,

        [Display(Name = "Истощенный")]
        Exhausted
    }
}
