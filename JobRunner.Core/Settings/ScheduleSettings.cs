using System;

namespace JobRunner.Core.Settings
{
    /// <summary>
    /// Периодичность задачи (когда выполнять)
    /// </summary>
    public class ScheduleSettings
    {
        /// <summary>
        /// Тип периодичности 
        /// </summary>
        public PeriodType PeriodType { get; set; } = PeriodType.Once;

        /// <summary>
        /// Интервал в минутах (для EveryMinutes)
        /// </summary>
        public int IntervalMinutes { get; set; } = 60;

        /// <summary>
        /// День месяца для ежемесячного запуска (1-31)
        /// </summary>
        public int MonthDay { get; set; } = 1;

        /// <summary>
        /// День недели для еженедельного запуска 
        /// (0 = воскресенье, 1 = понедельник и так далее)
        /// </summary>
        public DayOfWeek WeeklyDay { get; set; }

        /// <summary>
        /// Часы запуска (0-23)
        /// </summary>
        public int Hour { get; set; } = 0;

        /// <summary>
        /// Минуты запуска (0-59)
        /// </summary>
        public int Minute { get; set; } = 0;
    }
}
