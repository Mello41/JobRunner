using JobRunner.Core.Entities.ValueObjects;

namespace JobRunner.Core.Converters
{
    /// <summary>
    /// предназначен для преобразования объектов настроек расписания 
    /// в строку формата CRON — стандартного формата для задания 
    /// времени выполнения повторяющихся задач
    /// </summary>
    public interface ICronConverter
    {

        /// <summary>
        /// Принимает объект с настройками расписания и 
        /// возвращает валидную cron-строку (например, 
        /// "0 12 * * *" для ежедневного выполнения в полдень)
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        string Convert(IScheduleSettings settings);
    }
}
