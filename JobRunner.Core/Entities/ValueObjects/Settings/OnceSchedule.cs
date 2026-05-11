using System;

namespace JobRunner.Core.Entities.ValueObjects.Settings
{
    /// <summary>
    /// Однократный запуск в указанное время
    /// </summary>
    public sealed record OnceSchedule : IScheduleSettings
    {
        public DateTime StartTime { get; set; }

        public string GetDescription() =>
            $"Однократно {StartTime:dd.MM.yyyy HH:mm:ss}";

        public bool IsValid() => StartTime != default;

        /// <summary>
        /// Проверяет, нужно ли выполнять задачу
        /// </summary>
        /// <param name="isCompleted">Флаг из IJobTaskMetadata.IsCompleted</param>
        public bool ShouldExecute(bool isCompleted)
        {
            if (isCompleted) return false;
            return StartTime <= DateTime.Now;
        }
    }
}
