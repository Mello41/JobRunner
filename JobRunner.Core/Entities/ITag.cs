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
        /// Название метки 
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Описание метки
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Цвет метки в формате HEX ("#4CAF50")
        /// </summary>
        string Color { get; set; }

        /// <summary>
        /// Можно ли группировать по этой метке
        /// </summary>
        bool CanGrouping {  get; set; }
    }
}
