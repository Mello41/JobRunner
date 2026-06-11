namespace JobRunner.Core.Interfaces.Entities
{
    /// <summary>
    /// Отдельный аргумент командной строки для задачи
    /// </summary>
    /// <para><b>Связь с <see cref="ScheduleArgumentItem"/>:</b></para>
    /// <see cref="ScheduleArgumentItem"/> - это DTO для сериализации и UI.
    /// <c>IScheduleArgumentItem</c> - интерфейс для доменной модели.
    /// Они существуют отдельно, чтобы не смешивать слои.
    /// </remarks>
    public interface IScheduleArgumentItem
    {
        /// <summary>
        /// Ключ (имя) аргумента
        /// </summary>
        string Key { get; set; }

        /// <summary>
        /// Значение аргумента
        /// </summary>
        string Value { get; set; }

        /// <summary>
        /// Зашифрован аргумент?
        /// </summary>
        bool IsEncrypted { get; set; }
    }
}
