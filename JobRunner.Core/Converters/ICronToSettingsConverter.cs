using JobRunner.Core.Entities;

namespace JobRunner.Core.Converters
{
    /// <summary>
    /// Конвертер Cron-выражения в настройки периодичности
    /// </summary>
    public interface ICronToSettingsConverter
    {
        /// <summary>
        /// Преобразует Cron-строку в IScheduleSettings
        /// </summary>
        IScheduleSettings Convert(string cronExpression);
    }
}
