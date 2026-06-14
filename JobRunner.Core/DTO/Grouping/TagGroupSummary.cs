using System;
using System.ComponentModel.DataAnnotations;

namespace JobRunner.Core.DTO.Grouping
{
    /// <summary>
    /// Сводка по группе задач (метке)
    /// </summary>
    public class TagGroupSummary
    {
        [Display(Name = "Название метки")]
        public string TagName { get; set; } = string.Empty;

        [Display(Name = "Всего задач в группе")]
        public int TotalTasks { get; set; }

        [Display(Name = "Активных задач")]
        public int EnabledTasks { get; set; }

        [Display(Name = "Выполняется сейчас")]
        public int RunningTasks { get; set; }

        [Display(Name = "Процент успешных выполнений")]
        public double SuccessRatePercent { get; set; }

        [Display(Name = "Последний запуск группы")]
        public DateTime? LastGroupRun { get; set; }
    }
}