using JobRunner.Core.Entities.ValueObjects;
using JobRunner.Core.Entities.ValueObjects.Settings;
using JobRunner.Core.Interfaces.Converters;

namespace JobRunner.Quartz.Converters
{
    /// <summary>
    /// Конвертер IScheduleSettings → Cron-выражение для Quartz
    /// </summary>
    public class CronConverter : IScheduleConverter
    {
        /// <summary>
        /// Конвертировать IScheduleSettings в CRON выражение
        /// учет периодов по типам
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public string Convert(IScheduleSettings settings)
        {
            return settings switch
            {
                OnceSchedule once => ConvertOnce(once),
                DailySchedule daily => ConvertDaily(daily),
                WeeklySchedule weekly => ConvertWeekly(weekly),
                IntervalSchedule interval => ConvertInterval(interval),
                _ => throw new NotSupportedException($"Unsupported schedule type: {settings.GetType()}")
            };
        }

        #region Конвертация по типам 
        private string ConvertOnce(OnceSchedule once)
        {
            var start = once.StartTime;
            return $"{start.Second} {start.Minute} {start.Hour} {start.Day} {start.Month} ? {start.Year}";
        }

        private string ConvertDaily(DailySchedule daily)
        {
            return $"0 {daily.Minute} {daily.Hour} * * ?";
        }

        private string ConvertWeekly(WeeklySchedule weekly)
        {
            var days = string.Join(",", weekly.DaysOfWeek.Select(d => (int)d + 1));
            return $"0 {weekly.Minute} {weekly.Hour} ? * {days}";
        }

        private string ConvertInterval(IntervalSchedule interval)
        {
            return $"0 */{interval.IntervalMinutes} * * * ?";
        }
        #endregion
    }
}
