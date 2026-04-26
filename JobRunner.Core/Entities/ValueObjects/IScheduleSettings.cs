namespace JobRunner.Core.Entities.ValueObjects
{
    /// <summary>
    /// Маркерный интерфейс для всех типов расписаний
    /// </summary>
    /// <remarks>
    /// Реализации: OnceSchedule, IntervalSchedule, DailySchedule, WeeklySchedule, CronSchedule
    /// Полиморфная сериализация настраивается в Infrastructure слое через JsonDerivedTypeAttribute
    /// или через настройки JsonSerializerOptions.TypeInfoResolver
    /// </remarks>
    public interface IScheduleSettings
    {
        /// <summary>
        /// Преобразует расписание в Cron-выражение для Quartz
        /// </summary>
        /// <returns>Cron-выражение (формат: секунды минуты часы день 
        /// месяц день_недели год)</returns>
        string ToCronExpression();

        /// <summary>
        /// Возвращает человекочитаемое описание расписания
        /// </summary>
        /// <returns>Описание на русском языке</returns>
        string GetDescription();

        /// <summary>
        /// Проверяет, корректны ли параметры расписания
        /// </summary>
        /// <returns>true — расписание корректно, 
        /// false — ошибка в параметрах</returns>
        bool IsValid();
    }
}
