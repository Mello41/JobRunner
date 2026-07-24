using JobRunner.Core.Interfaces.Entities.JobTaskSettings.ScheduleSettings;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace JobRunner.Core.Models.ValueSchedule
{
    /// <summary>
    /// Расписание "Каждый квартал"
    /// Quarter = 1, 2, 3, 4
    /// </summary>
    public sealed record QuarterlySchedule : IScheduleSettings
    {
        [Display(Name = "Номер квартала")]
        [Range(1, 4, ErrorMessage = "Номер квартала должен быть от 1 до 4")]
        public int Quarter { get; set; } = 1;  // ← было StartMonth

        [Display(Name = "День месяца")]
        [Range(1, 31, ErrorMessage = "День должен быть от 1 до 31")]
        public int Day { get; set; } = 1;

        [Display(Name = "Час")]
        [Range(0, 23, ErrorMessage = "Час должен быть от 0 до 23")]
        public int Hour { get; set; } = 9;

        [Display(Name = "Минута")]
        [Range(0, 59, ErrorMessage = "Минута должна быть от 0 до 59")]
        public int Minute { get; set; } = 0;

        public string GetDescription()
        {
            var sb = new StringBuilder();
            sb.Append("Каждый квартал ");

            var quarterName = Quarter switch
            {
                1 => "(I квартал: январь-март)",
                2 => "(II квартал: апрель-июнь)",
                3 => "(III квартал: июль-сентябрь)",
                4 => "(IV квартал: октябрь-декабрь)",
                _ => $"({Quarter} квартал)"
            };

            sb.Append(quarterName);
            sb.Append($", {Day:D2} числа в {Hour:D2}:{Minute:D2}");

            return sb.ToString();
        }

        public bool IsValid()
        {
            if (Minute < 0 || Minute > 59) return false;
            if (Hour < 0 || Hour > 23) return false;
            if (Day < 1 || Day > 31) return false;
            if (Quarter < 1 || Quarter > 4) return false;

            return true;
        }

        /// <summary>
        /// Получить список месяцев для квартала
        /// </summary>
        public int[] GetMonths()
        {
            return Quarter switch
            {
                1 => new[] { 1, 2, 3 },
                2 => new[] { 4, 5, 6 },
                3 => new[] { 7, 8, 9 },
                4 => new[] { 10, 11, 12 },
                _ => new[] { 1, 4, 7, 10 }
            };
        }
    }
}
