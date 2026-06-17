using JobRunner.Core.Interfaces.Entities.JobTaskSettings.ScheduleSettings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JobRunner.Core.Entities.ValueObjects
{
    /// <summary>
    /// Расписание для еженедельного выполнения задачи
    /// </summary>
    public sealed record WeeklySchedule : IScheduleSettings
    {
        /// <summary>
        /// Часы запуска (0-23)
        /// </summary>
        public int Hour { get; set; }

        /// <summary>
        /// Минуты запуска (0-59)
        /// </summary>
        public int Minute { get; set; }

        /// <summary>
        /// Дни недели для запуска
        /// </summary>
        public HashSet<DayOfWeek> DaysOfWeek { get; set; } = new();

        public string GetDescription()
        {
            var days = string.Join(", ", DaysOfWeek.Select(GetShortDayName));
            return $"Еженедельно в {Hour:D2}:{Minute:D2} ({days})";
        }

        public bool IsValid() => 
            DaysOfWeek.Count > 0 && Minute >= 0 && Minute <= 59 && Hour >= 0 && Hour <= 23;

        private static string GetShortDayName(DayOfWeek day) => day switch
        {
            DayOfWeek.Monday => "ПН",
            DayOfWeek.Tuesday => "ВТ",
            DayOfWeek.Wednesday => "СР",
            DayOfWeek.Thursday => "ЧТ",
            DayOfWeek.Friday => "ПТ",
            DayOfWeek.Saturday => "СБ",
            DayOfWeek.Sunday => "ВС",
            _ => day.ToString()
        };
    }
}
