using System;

namespace JobRunner.Core.Entities.ValueObjects.Settings
{
    /// <summary>
    /// Расписание для периодического выполнения задачи с фиксированным интервалом
    /// </summary>
    public sealed record IntervalSchedule : IScheduleSettings
    {
        /// <summary>
        /// Интервал между запусками
        /// </summary>
        public int IntervalMinutes { get; set; } = 60;

        /// <summary>
        /// Опциональное время первого запуска (если не указано — начинается с момента создания)
        /// </summary>
        public DateTime? StartAt { get; set; }

        public string ToCronExpression()
        {
            var minutes = (int)IntervalMinutes;
            minutes = Clamp(minutes, 1, 59);
            return $"0 */{minutes} * * * ?";
        }

        public string GetDescription() => $"Каждые {(int)IntervalMinutes} минут";

        public bool IsValid() => IntervalMinutes >= 1 && IntervalMinutes <= 59;

        private static int Clamp(int value, int min, int max)
        {
            return value < min ? min : (value > max ? max : value);
        }

    }
}
