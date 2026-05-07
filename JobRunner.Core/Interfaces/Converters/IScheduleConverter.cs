using JobRunner.Core.Entities.ValueObjects;

namespace JobRunner.Core.Interfaces.Converters
{
    /// <summary>
    /// 
    /// </summary>
    public interface IScheduleConverter
    {
        /// <summary>
        /// Преобразует настройки в формат, понятный планировщику
        /// (для Quartz → Cron, для FluentScheduler → интервал и т.д.)
        /// </summary>
        string Convert(IScheduleSettings settings);
    }
}
