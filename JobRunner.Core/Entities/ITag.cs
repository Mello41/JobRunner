using System;

namespace JobRunner.Core.Entities
{
    /// <summary>
    /// Интерфейс метки (категории) задачи
    /// </summary>
    public interface ITag
    {
        /// <summary>
        /// Уникальный идентификатор метки
        /// </summary>
        Guid Id { get; set; }

        /// <summary>
        /// Название метки (например, "Работа", "Срочно")
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Цвет метки в формате HEX (например, "#4CAF50")
        /// </summary>
        string Color { get; set; }

        /// <summary>
        /// Можно ли группировать
        /// </summary>
        bool CanGrooping {  get; set; }
    }
}
