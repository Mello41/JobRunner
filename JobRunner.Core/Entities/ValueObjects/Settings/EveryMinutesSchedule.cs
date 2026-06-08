using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace JobRunner.Core.Entities.ValueObjects.Settings
{
    /// <summary>
    /// Расписание для выполнения с фиксированным интервалом в минутах
    /// </summary>
    public sealed record EveryMinutesSchedule : IScheduleSettings
    {
        private int _intervalMinutes = 5;

        /// <summary>
        /// Интервал в минутах (1-59)
        /// </summary>
        [DisplayName("Интервал (минут)")]
        [Description("Периодичность выполнения задачи в минутах")]
        [Range(1, 59, ErrorMessage = "Интервал должен быть от 1 до 59 минут")]
        public int IntervalMinutes
        {
            get => _intervalMinutes;
            set => _intervalMinutes = Clamp(value, 1, 59);
        }

        /// <summary>
        /// Опциональное время первого запуска
        /// Если не указано - начинается с момента создания/старта
        /// </summary>
        [DisplayName("Время первого запуска")]
        [Description("Опциональное время первого запуска. Если не указано - начинается с текущего момента")]
        public DateTime? StartAt { get; set; }

        /// <summary>
        /// Опциональное время окончания действия расписания
        /// </summary>
        [DisplayName("Время окончания")]
        [Description("После указанного времени задача больше не будет выполняться")]
        public DateTime? EndAt { get; set; }

        /// <summary>
        /// Возвращает человекочитаемое описание расписания
        /// </summary>
        public string GetDescription()
        {
            var interval = IntervalMinutes switch
            {
                1 => "каждую минуту",
                2 => "каждые 2 минуты",
                3 => "каждые 3 минуты",
                4 => "каждые 4 минуты",
                _ => $"каждые {IntervalMinutes} минут"
            };

            var startPart = StartAt.HasValue
                ? $", начиная с {StartAt.Value:HH:mm:ss}"
                : string.Empty;

            var endPart = EndAt.HasValue
                ? $", до {EndAt.Value:HH:mm:ss}"
                : string.Empty;

            return $"{interval}{startPart}{endPart}";
        }

        /// <summary>
        /// Проверяет, корректны ли параметры расписания
        /// </summary>
        public bool IsValid()
        {
            if (IntervalMinutes < 1 || IntervalMinutes > 59)
                return false;

            if (StartAt.HasValue && EndAt.HasValue && StartAt.Value >= EndAt.Value)
                return false;

            return true;
        }

        /// <summary>
        /// Рассчитывает следующее время выполнения
        /// </summary>
        /// <param name="fromTime">Время, от которого рассчитываем</param>
        /// <returns>Следующее время выполнения или null, если расписание закончилось</returns>
        public DateTime? GetNextRunTime(DateTime fromTime)
        {
            if (EndAt.HasValue && fromTime >= EndAt.Value)
                return null;

            DateTime nextRun;

            if (StartAt.HasValue && fromTime < StartAt.Value)
            {
                nextRun = StartAt.Value;
            }
            else
            {
                var minutesSinceStart = (int)(fromTime - (StartAt ?? DateTime.MinValue)).TotalMinutes;
                var minutesToNext = IntervalMinutes - (minutesSinceStart % IntervalMinutes);
                nextRun = fromTime.AddMinutes(minutesToNext);
            }

            nextRun = new DateTime(nextRun.Year, nextRun.Month, nextRun.Day,
                                   nextRun.Hour, nextRun.Minute, 0, 0);

            if (EndAt.HasValue && nextRun >= EndAt.Value)
                return null;

            return nextRun;
        }

        /// <summary>
        /// Ограничивает значение указанными пределами
        /// </summary>
        private static int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }
}
