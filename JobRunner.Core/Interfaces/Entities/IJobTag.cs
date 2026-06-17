using System;
using System.Collections.Generic;

namespace JobRunner.Core.Interfaces.Entities
{
    /// <summary>
    /// Интерфейс метки (категории) задачи
    /// </summary>
    public interface IJobTag<TId> where TId : IEquatable<TId>
    {
        /// <summary>
        /// Уникальный идентификатор метки
        /// </summary>
        TId Id { get; set; }

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
        string? Color { get; set; }

        /// <summary>
        /// Можно ли группировать по этой метке
        /// </summary>
        bool CanGrouping {  get; set; }

        /// <summary>
        /// Можно ли запускать все задачи этой метки одной командой
        /// </summary>
        bool AllowGroupRun { get; set; }

        /// <summary>
        /// Можно ли приостанавливать все задачи этой метки
        /// </summary>
        bool AllowGroupPause { get; set; }

        /// <summary>
        /// Задачи в метке с порядком. Использует SortedSet с кастомным компаратором.
        /// </summary>
        SortedSet<TId>? OrderedTaskIds { get; set; }
    }
}
