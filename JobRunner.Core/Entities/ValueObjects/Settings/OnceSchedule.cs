using System;

namespace JobRunner.Core.Entities.ValueObjects.Settings
{
    /// <summary>
    /// Однократный запуск в указанное время
    /// </summary>
    public sealed record OnceSchedule : IScheduleSettings
    {
        /// <summary>
        /// Дата и время запуска
        /// </summary>
        public DateTime StartTime { get; } // set --> init;

        /// <summary>
        /// Конвертация в крон формат 
        /// Quartz cron формат: секунды минуты часы день месяц ? год
        /// </summary>
        /// <returns></returns>
        public string ToCronExpression()
        {
            return $"{StartTime.Second} {StartTime.Minute} {StartTime.Hour} {StartTime.Day} {StartTime.Month} ? {StartTime.Year}";
        }

        public string GetDescription()
        {
            return $"Однократно {StartTime:dd.MM.yyyy HH:mm:ss}";
        }

        public bool IsValid()
        {
            return StartTime > DateTime.Now;
        }
    }
}
