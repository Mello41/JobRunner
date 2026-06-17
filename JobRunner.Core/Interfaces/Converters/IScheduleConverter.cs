using JobRunner.Core.Interfaces.Entities.JobTaskSettings.ScheduleSettings;

namespace JobRunner.Core.Interfaces.Converters
{
    /// <summary>
    /// Конвертирует IScheduleSettings в строку для планировщика
    /// ключевой компонент для интеграции JobRunner с 
    /// Quartz.NET. или другой библиотекой
    /// Без него планировщик не сможет понять, 
    /// когда запускать задачи
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