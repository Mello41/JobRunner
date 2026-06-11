using JobRunner.Core.Interfaces.Entities.JobTaskSettings;
using System;

namespace JobRunner.Core.Entities.ValueObjects
{
    /// <summary>
    /// Расписание для периодического выполнения
    /// задачи с фиксированным интервалом
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

        /// <summary>
        /// Возвращает человекочитаемое описание расписания
        /// </summary>
        /// <returns>
        public string GetDescription() => $"Каждые {(int)IntervalMinutes} минут";

        /// <summary>
        /// Проверяет, корректны ли параметры расписания
        /// </summary>
        /// <returns></returns>
        public bool IsValid() => IntervalMinutes >= 1 && IntervalMinutes <= 59;
    }
}
