using JobRunner.Core.Interfaces.Entities.JobTaskSettings.ScheduleSettings;
using System;

namespace JobRunner.Core.Models.ValueSchedule
{
    /// <summary>
    /// Расписание для ежегодного выполнения
    /// </summary>
    public sealed record YearlySchedule : IScheduleSettings
    {
        /// <summary>
        /// Месяц (1-12)
        /// </summary>
        public int Month { get; set; } = 1;

        /// <summary>
        /// День месяца (1-31)
        /// </summary>
        public int Day { get; set; } = 1;

        /// <summary>
        /// Час запуска (0-23)
        /// </summary>
        public int Hour { get; set; } = 0;

        /// <summary>
        /// Минута запуска (0-59)
        /// </summary>
        public int Minute { get; set; } = 0;

        /// <summary>
        /// Секунда запуска (0-59)
        /// </summary>
        public int Seconds { get; set; } = 0;

        public string GetDescription()
        {
            return $"Ежегодно {Day:00}.{Month:00} в {Hour:D2}:{Minute:D2}:{Seconds:D2}";
        }

        /// <summary>
        /// Проверка корректности дня для месяца (например, 31 апреля — невалидно)
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            if (Month < 1 || Month > 12) return false;
            if (Day < 1 || Day > 31) return false;
            if (Hour < 0 || Hour > 23) return false;
            if (Minute < 0 || Minute > 59) return false;
            if (Seconds < 0 || Seconds > 59) return false;

            if (Day > DateTime.DaysInMonth(DateTime.Now.Year, Month))
                return false;

            return true;
        }
    }
}