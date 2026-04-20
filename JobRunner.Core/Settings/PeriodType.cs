using System.ComponentModel.DataAnnotations;

namespace JobRunner.Core.Settings
{
    /// <summary>
    /// Список для периодичности задачи Jobtask
    /// </summary>
    public enum PeriodType
    {
        [Display(Name = "Однократно")]
        Once,

        [Display(Name = "Каждую минуту")]
        EveryMinutes,

        [Display(Name = "Кадый час")]
        EveryHourly,

        [Display(Name = "Каждый день")]
        EveryDaily,

        [Display(Name = "Каждую неделю")]
        EveryWeekly,

        [Display(Name = "Ежемесячно")]
        EveryMonthly,

        [Display(Name = "Каждый квартал")]
        EveryQuarterly,

        [Display(Name = "Каждый год")]
        EveryYearly
    }
}
