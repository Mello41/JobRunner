using JobRunner.Core.Interfaces.Entities.JobTaskSettings;
using System.ComponentModel;

namespace JobRunner.Core.Entities.ValueObjects
{
    /// <summary>
    /// Расписание для ежедневного выполнения
    /// </summary>
    public sealed record DailySchedule : IScheduleSettings
    {
        [DisplayName("")]
        public int Hour { get; set; }

        [DisplayName("")]
        public int Minute { get; set; }

        /// <summary>
        /// Возвращает человеко-читаемое описание расписания задачи
        /// </summary>
        /// <returns></returns>
        public string GetDescription() => $"Ежедневно в {Hour:D2}:{Minute:D2}";

        /// <summary>
        /// Проверяет, корректны ли параметры расписания
        /// </summary>
        /// <returns></returns>
        public bool IsValid() => Minute >= 0 && Minute <= 59 && Hour >= 0 && Hour <= 23;
    
    }
}
