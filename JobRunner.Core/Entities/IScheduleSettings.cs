using JobRunner.Core.Settings;
using System;

namespace JobRunner.Core.Entities
{
    /// <summary>
    /// Интерфейс настроек периодичности задачи
    /// Формально, этот формат не привязан к CRON 
    /// (можно использовать для чего-нибудь другого)
    /// </summary>
    public interface IScheduleSettings 
    {
        /// <summary>
        /// Тип периодичности
        /// </summary>
        PeriodType PeriodType { get; set; }

        /// <summary>
        /// Интервал в минутах (для EveryMinutes)
        /// </summary>
        int IntervalMinutes { get; set; }

        /// <summary>
        /// День месяца для ежемесячного запуска (1-31)
        /// </summary>
        int MonthDay { get; set; }

        /// <summary>
        /// День недели для еженедельного запуска
        /// </summary>
        DayOfWeek WeeklyDay { get; set; }

        /// <summary>
        /// Часы запуска (0-23)
        /// </summary>
        int Hour { get; set; }

        /// <summary>
        /// Минуты запуска (0-59)
        /// </summary>
        int Minute { get; set; }
    }
}
