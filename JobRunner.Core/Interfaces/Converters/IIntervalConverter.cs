using JobRunner.Core.Entities.ValueObjects;
using System;

namespace JobRunner.Core.Interfaces.Converters
{
    /// <summary>
    /// Интерфейс конвертера интервала для других вариантов библиотек
    /// (для Timer/FluentScheduler)
    /// работает с TimeSpan интервалами
    /// </summary>
    public interface IIntervalConverter
    {
        /// <summary>
        /// Преобразует IScheduleSettings в TimeSpan интервал
        /// </summary>
        TimeSpan Convert(IScheduleSettings settings);

        /// <summary>
        /// Проверяет, можно ли представить настройки как простой интервал
        /// </summary>
        bool CanConvert(IScheduleSettings settings);
    }
}
