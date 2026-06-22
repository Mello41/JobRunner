using System;
using System.Collections.Generic;

namespace JobRunner.Core.DTO.Grouping
{
    /// <summary>
    /// Обновление приоритетов задач в группе (для drag-n-drop)
    /// </summary>
    public class TaskPriorityUpdate<TId> where TId : IEquatable<TId>
    {
        /// <summary>
        /// ID группы (метки)
        /// </summary>
        public TId GroupId { get; set; } = default!;

        /// <summary>
        /// Список ID задач в новом порядке
        /// </summary>
        public List<TId> TaskIdsInOrder { get; set; } = new();

        /// <summary>
        /// ID задачи, после которой нужно вставить (null = в начало)
        /// </summary>
        public TId? InsertAfterTaskId { get; set; }

        /// <summary>
        /// Переместить задачу вверх (true) или вниз (false)
        /// </summary>
        public bool MoveUp { get; set; }
    }
}
