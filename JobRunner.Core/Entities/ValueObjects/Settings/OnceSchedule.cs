using System;

namespace JobRunner.Core.Entities.ValueObjects.Settings
{
    /// <summary>
    /// Однократный запуск в указанное время
    /// </summary>
    public sealed record OnceSchedule : IScheduleSettings
    {
        public DateTime StartTime { get; init; }

        /// <summary>
        /// dd.MM.yyyy HH:mm:ss
        /// </summary>
        /// <returns></returns>
        public string GetDescription() => $"Однократно {StartTime:dd.MM.yyyy HH:mm:ss}";

        public bool IsValid() => StartTime > DateTime.Now;
    }
}
