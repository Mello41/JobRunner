namespace JobRunner.Core.Entities.ValueObjects.Settings
{
    /// <summary>
    /// Ежемесячное расписание
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

        public string ToCronExpression()
        {
            if (Month.HasValue && Month.Value >= 1 && Month.Value <= 12)
                return $"0 {Minute} {Hour} {Day} {Month.Value} ?";

            return $"0 {Minute} {Hour} {Day} * ?";
        }

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

            if (Month.HasValue && Day > DaysInMonth(Month.Value))
                return false;

            return true;
        }

        private int DaysInMonth(int month)
        {
            return month switch
            {
                2 => 28, // Февраль без 29 дней
                4 or 6 or 9 or 11 => 30,
                _ => 31
            };
        }

        private string GetMonthName(int month)
        {
            return month switch
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
}
