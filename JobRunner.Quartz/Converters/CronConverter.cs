using JobRunner.Core.Converters;
using JobRunner.Core.Entities;
using JobRunner.Core.Settings;

namespace JobRunner.Quartz.Converters
{
    /// <summary>
    /// Конвертер IScheduleSettings → Cron-выражение для библиотеки Quartz
    /// необходим удобный формат для ввода (как по кнопке в интерфейсе)
    /// </summary>
    public class CronConverter : ICronConverter
    {
        public string Convert(IScheduleSettings settings)
        {
            return settings.PeriodType switch
            {
                PeriodType.Once => string.Empty,
                PeriodType.EveryMinutes => $"0 */{settings.IntervalMinutes} * * * ?",
                PeriodType.EveryHourly => $"0 {settings.Minute} * * * ?",
                PeriodType.EveryDaily => $"0 {settings.Minute} {settings.Hour} * * ?",
                PeriodType.EveryWeekly => $"0 {settings.Minute} {settings.Hour} ? * {(int)settings.WeeklyDay + 1}",
                PeriodType.EveryMonthly => $"0 {settings.Minute} {settings.Hour} {settings.MonthDay} * ?",
                PeriodType.EveryQuarterly => $"0 {settings.Minute} {settings.Hour} {settings.MonthDay} 1,4,7,10 ?",
                PeriodType.EveryYearly => $"0 {settings.Minute} {settings.Hour} {settings.MonthDay} 1 ?",
                _ => throw new NotSupportedException($"PeriodType {settings.PeriodType} не поддерживается")
            };
        }
    }
}
