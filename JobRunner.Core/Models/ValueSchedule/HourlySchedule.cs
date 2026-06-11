using JobRunner.Core.Interfaces.Entities.JobTaskSettings;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace JobRunner.Core.Entities.ValueObjects
{
    /// <summary>
    /// Расписание для ежечасного выполнения задачи
    /// </summary>
    public sealed record HourlySchedule : IScheduleSettings
    {
        private int _hourInterval = 1;
        private int _minute = 0;

        /// <summary>
        /// Интервал в часах (1-23)
        /// </summary>
        [DisplayName("Интервал (часов)")]
        [Description("Периодичность выполнения задачи в часах")]
        [Range(1, 23, ErrorMessage = "Интервал должен быть от 1 до 23 часов")]
        public int HourInterval
        {
            get => _hourInterval;
            set => _hourInterval = Clamp(value, 1, 23);
        }

        /// <summary>
        /// Минуты запуска (0-59)
        /// </summary>
        [DisplayName("Минуты")]
        [Description("Минуты, в которые будет запускаться задача")]
        [Range(0, 59, ErrorMessage = "Минуты должны быть от 0 до 59")]
        public int Minute
        {
            get => _minute;
            set => _minute = Clamp(value, 0, 59);
        }

        /// <summary>
        /// Опциональное время первого запуска
        /// </summary>
        [DisplayName("Время первого запуска")]
        [Description("Опциональное время первого запуска")]
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
            var interval = HourInterval switch
            {
                1 => "каждый час",
                2 => "каждые 2 часа",
                3 => "каждые 3 часа",
                _ => $"каждые {HourInterval} часов"
            };

            var minutePart = Minute == 0
                ? "в 00 минут"
                : $"в {Minute:00} минут";

            var startPart = StartAt.HasValue
                ? $", начиная с {StartAt.Value:HH:mm:ss}"
                : string.Empty;

            var endPart = EndAt.HasValue
                ? $", до {EndAt.Value:HH:mm:ss}"
                : string.Empty;

            return $"{interval} {minutePart}{startPart}{endPart}";
        }

        /// <summary>
        /// Проверяет, корректны ли параметры расписания
        /// </summary>
        public bool IsValid()
        {
            if (HourInterval < 1 || HourInterval > 23)
                return false;

            if (Minute < 0 || Minute > 59)
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
                // Округляем до часа с учетом минут
                var baseHour = new DateTime(fromTime.Year, fromTime.Month, fromTime.Day,
                                           fromTime.Hour, Minute, 0);

                if (fromTime <= baseHour)
                {
                    nextRun = baseHour;
                }
                else
                {
                    var hoursToAdd = HourInterval - ((fromTime.Hour - baseHour.Hour) % HourInterval);
                    nextRun = baseHour.AddHours(hoursToAdd);
                }
            }

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
