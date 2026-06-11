using JobRunner.Core.Interfaces.Entities.JobTaskSettings;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace JobRunner.Core.Entities.ValueObjects
{
    /// <summary>
    /// Расписание "Каждый квартал"
    /// </summary>
    public sealed record QuarterlySchedule : IScheduleSettings
    {
        [Display(Name = "Месяц начала квартала")]
        public int StartMonth { get; set; } = 1; 

        [Display(Name = "День месяца")]
        public int Day { get; set; } = 1;

        [Display(Name = "Час")]
        public int Hour { get; set; } = 9;

        [Display(Name = "Минута")]
        public int Minute { get; set; } = 0;

        /// <summary>
        /// Возвращает человеко-читаемое описание расписания задачи
        /// </summary>
        public string GetDescription()
        {
            var sb = new StringBuilder();

            sb.Append("Каждый квартал, ");

            var quarterName = StartMonth switch
            {
                1 => "I квартал (январь-март)",
                4 => "II квартал (апрель-июнь)",
                7 => "III квартал (июль-сентябрь)",
                10 => "IV квартал (октябрь-декабрь)",
                _ => $"{StartMonth} месяц (начало квартала)"
            };

            sb.Append(quarterName);
            sb.Append($", {Day:D2} числа");

            if (Day == 31)
                sb.Append(" (последний день месяца)");

            sb.Append($" в {Hour:D2}:{Minute:D2}");

            return sb.ToString();
        }

        /// <summary>
        /// Проверяет, корректны ли параметры расписания
        /// </summary>
        public bool IsValid()
        {
            if (Minute < 0 || Minute > 59) return false;
            if (Hour < 0 || Hour > 23) return false;

            if (Day < 1 || Day > 31) return false;

            if (StartMonth != 1 && StartMonth != 4 && StartMonth != 7 && StartMonth != 10)
                return false;

            return true;
        }

    }
}
