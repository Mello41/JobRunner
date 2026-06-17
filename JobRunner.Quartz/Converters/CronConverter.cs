using JobRunner.Core.Entities.ValueObjects;
using JobRunner.Core.Interfaces.Converters;
using JobRunner.Core.Interfaces.Entities.JobTaskSettings.ScheduleSettings;

namespace JobRunner.Quartz.Converters
{
    /// <summary>
    /// Конвертер IScheduleSettings → Cron-выражение для Quartz
    /// </summary>
    public class CronConverter : IScheduleConverter
    {
        /// <summary>
        /// Конвертировать IScheduleSettings в CRON выражение
        /// учет периодов по типам
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public string Convert(IScheduleSettings settings)
        {
            ArgumentNullException.ThrowIfNull(settings);

            return settings switch
            {
                OnceSchedule once => ConvertOnce(once),
                DailySchedule daily => ConvertDaily(daily),
                WeeklySchedule weekly => ConvertWeekly(weekly),
                IntervalSchedule interval => ConvertInterval(interval),
                MonthlySchedule monthly => ConvertMonthly(monthly),
                QuarterlySchedule quarterly => ConvertQuarterly(quarterly),
                YearlySchedule yearly => ConvertYearly(yearly),
                EveryMinutesSchedule everyMinutes => ConvertEveryMinutes(everyMinutes),
                HourlySchedule hourly => ConvertHourly(hourly),
                _ => throw new NotSupportedException($"Unsupported schedule type: {settings.GetType()}")
            };
        }

        #region Конвертация по типам в PeriodType

        /// <summary>
        /// 
        /// </summary>
        /// <param name="everyMinutes"></param>
        /// <returns></returns>
        private string ConvertEveryMinutes(EveryMinutesSchedule everyMinutes)
        {
            return $"0 */{everyMinutes.IntervalMinutes} * * * ?";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hourly"></param>
        /// <returns></returns>
        private string ConvertHourly(HourlySchedule hourly)
        {
            if (hourly.HourInterval == 1)
                return $"0 {hourly.Minute} * * * ?";

            return $"0 {hourly.Minute} */{hourly.HourInterval} * * ?";
        }

        /// <summary>
        /// Метод конвертации для задачи 
        /// </summary>
        /// <param name="once"></param>
        /// <returns></returns>
        /// <remarks>
        /// Quartz cron expression НЕ поддерживает
        /// секунды в стандартном формате 
        /// (только 6 полей: минуты, часы, день,
        /// месяц, день_недели, год)
        /// </remarks>
        private string ConvertOnce(OnceSchedule once)
        {
            var start = once.StartTime;
            return $"{start.Minute} {start.Hour} {start.Day} {start.Month} ? {start.Year}";
        }

        private string ConvertDaily(DailySchedule daily)
        {
            return $"0 {daily.Minute} {daily.Hour} * * ?";
        }

        private string ConvertWeekly(WeeklySchedule weekly)
        {
            var days = string.Join(",", weekly.DaysOfWeek.Select(d => (int)d + 1));
            return $"0 {weekly.Minute} {weekly.Hour} ? * {days}";
        }

        private string ConvertMonthly(MonthlySchedule monthly)
        {
            return $"0 {monthly.Minute} {monthly.Hour} {monthly.Day} * ?";
        }

        /// <summary>
        /// Квартал: январь(1), апрель(4), июль(7), октябрь(10)
        /// Cron: "0 минут час день месяц ?"
        /// Для квартала нужно указать несколько месяцев через запятую
        /// </summary>
        /// <param name="quarterly"></param>
        /// <returns></returns>
        private string ConvertQuarterly(QuarterlySchedule quarterly)
        {
            var months = GetQuarterMonths(quarterly.StartMonth);
            return $"0 {quarterly.Minute} {quarterly.Hour} {quarterly.Day} {months} ?";
        }

        private string ConvertYearly(YearlySchedule yearly)
        {
            return $"0 {yearly.Minute} {yearly.Hour} {yearly.Day} {yearly.Month} ?";
        }

        /// <summary>
        /// Конвертация по простым типам периодичности
        /// (MinuteConverter и HourlyConverter)
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        private string ConvertInterval(IntervalSchedule interval)
        {
            return $"0 */{interval.IntervalMinutes} * * * ?";
        }
        #endregion

        /// <summary>
        /// Вспомогательный метод для ConvertQuarterly
        /// </summary>
        /// <param name="startMonth"></param>
        /// <returns></returns>
        private string GetQuarterMonths(int startMonth)
        {
            return startMonth switch
            {
                1 => "1,2,3",     // Q1: январь, февраль, март
                4 => "4,5,6",     // Q2: апрель, май, июнь
                7 => "7,8,9",     // Q3: июль, август, сентябрь
                10 => "10,11,12", // Q4: октябрь, ноябрь, декабрь
                _ => "1,4,7,10"   // по умолчанию все кварталы
            };
        }
    }
}
