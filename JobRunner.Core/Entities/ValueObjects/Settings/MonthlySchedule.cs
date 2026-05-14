using System;

namespace JobRunner.Core.Entities.ValueObjects.Settings
{
    /// <summary>
    /// Расписание для ежемесячного выполнения
    /// </summary>
    public class MonthlySchedule : IScheduleSettings
    {
        /// <summary>
        /// День месяца (1-31)
        /// </summary>
        public int Day { get; set; } = 1;

        /// <summary>
        /// Месяц (1-12, опционально)
        /// </summary>
        public int? Month { get; set; }

        /// <summary>
        /// Час (0-23)
        /// </summary>
        public int Hour { get; set; } = 0;

        /// <summary>
        /// Минута (0-59)
        /// </summary>
        public int Minute { get; set; } = 0;

        public string GetDescription()
        {
            var monthPart = Month.HasValue ? $" в {GetMonthName(Month.Value)}" : "";
            return $"Ежемесячно {Day} числа{monthPart} в {Hour:D2}:{Minute:D2}";
        }

        public bool IsValid()
        {
            if (Hour < 0 || Hour > 23) return false;
            if (Minute < 0 || Minute > 59) return false;
            if (Day < 1 || Day > 31) return false;
            if (Month.HasValue && (Month.Value < 1 || Month.Value > 12)) return false;

            // Проверка на 31 февраля и т.д.
            if (Month.HasValue && Day > DateTime.DaysInMonth(DateTime.Now.Year, Month.Value))
                return false;

            return true;
        }

        private string GetMonthName(int month) => month switch
        {
            1 => "январе",
            2 => "феврале",
            3 => "марте",
            4 => "апреле",
            5 => "мае",
            6 => "июне",
            7 => "июле",
            8 => "августе",
            9 => "сентябре",
            10 => "октябре",
            11 => "ноябре",
            12 => "декабре",
            _ => ""
        };
    }
}
