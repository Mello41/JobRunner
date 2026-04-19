using JobRunner.Core.Settings;
using Microsoft.Win32.TaskScheduler;

namespace JobRunner.WindowsService.Converters
{
    /// <summary>
    /// Конвертер настроек расписания в триггер 
    /// Windows Task Scheduler (планировщик заданий Windows)
    /// </summary>
    public static class TriggerConverter
    {
        /// <summary>
        /// Преобразование настроек расписания задачи 
        /// в триггер Windows Task Scheduler
        /// </summary>
        /// <param name="settings"> Настройки распиания задачи </param>
        /// <returns> Триггер или null, если param settings == null </returns>
        public static Trigger? Convert(ScheduleSettings? settings)
        {
            if (settings == null) return null;

            return settings.PeriodType switch
            {
                PeriodType.Once => new TimeTrigger
                {
                    StartBoundary = DateTime.Now
                },

                PeriodType.EveryMinutes => new DailyTrigger
                {
                    DaysInterval = 1,
                    StartBoundary = DateTime.Today.Add(new TimeSpan(settings.Hour, settings.Minute, 0))
                },

                PeriodType.EveryHourly => new DailyTrigger
                {
                    DaysInterval = 1,
                    StartBoundary = DateTime.Today.Add(new TimeSpan(settings.Hour, settings.Minute, 0))
                },

                PeriodType.EveryDaily => new DailyTrigger
                {
                    DaysInterval = 1,
                    StartBoundary = DateTime.Today.Add(new TimeSpan(settings.Hour, settings.Minute, 0))
                },

                PeriodType.EveryWeekly => new WeeklyTrigger
                {
                    WeeksInterval = 1,
                    StartBoundary = DateTime.Today.Add(new TimeSpan(settings.Hour, settings.Minute, 0))
                },

                _ => new TimeTrigger()
            };
        }
    }
}
