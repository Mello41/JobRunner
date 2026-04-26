using JobRunner.Core.Converters;
using JobRunner.Core.Entities.ValueObjects;

namespace JobRunner.Quartz.Converters
{
    /// <summary>
    /// Конвертер IScheduleSettings → Cron-выражение для Quartz
    /// </summary>
    public class CronConverter : ICronConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public string Convert(IScheduleSettings settings)
        {
            return settings.ToCronExpression();
        }
    }
}
