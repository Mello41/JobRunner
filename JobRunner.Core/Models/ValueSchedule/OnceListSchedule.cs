using JobRunner.Core.Interfaces.Entities.JobTaskSettings.ScheduleSettings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JobRunner.Core.Models.ValueSchedule
{
    /// <summary>
    /// Расписание для списка однократных выполнений
    /// </summary>
    public sealed record OnceListSchedule : IScheduleSettings
    {
        /// <summary>
        /// Список дат для выполнения
        /// </summary>
        public List<DateTime> Dates { get; set; } = new();

        /// <summary>
        /// Возвращает человеко-читаемое описание расписания
        /// </summary>
        public string GetDescription()
        {
            if (Dates == null || Dates.Count == 0)
                return "Нет запланированных дат";

            var count = Dates.Count;
            var next = Dates.OrderBy(d => d).FirstOrDefault();
            var last = Dates.OrderBy(d => d).LastOrDefault();

            if (count == 1)
                return $"Однократно {next:dd.MM.yyyy HH:mm:ss}";

            return $"{count} выполнений: от {next:dd.MM.yyyy} до {last:dd.MM.yyyy}";
        }

        /// <summary>
        /// Проверяет, корректны ли параметры расписания
        /// </summary>
        public bool IsValid()
        {
            return Dates != null && Dates.Count > 0 && Dates.All(d => d > DateTime.Now);
        }

        /// <summary>
        /// Получить следующую дату для выполнения
        /// </summary>
        public DateTime? GetNextRunTime(DateTime fromTime)
        {
            return Dates?
                .Where(d => d > fromTime)
                .OrderBy(d => d)
                .FirstOrDefault();
        }

        /// <summary>
        /// Отметить дату как выполненную (удалить из списка)
        /// </summary>
        public void MarkAsCompleted(DateTime dateTime)
        {
            Dates.Remove(dateTime);
        }

        /// <summary>
        /// Получить количество запланированных выполнений
        /// </summary>
        public int PendingCount => Dates?.Count(d => d > DateTime.Now) ?? 0;

        /// <summary>
        /// Получить количество выполненных
        /// </summary>
        public int CompletedCount => Dates?.Count(d => d <= DateTime.Now) ?? 0;
    }
}
