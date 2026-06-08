using JobRunner.Core.Interfaces.Events.TaskCrud;
using System;

namespace JobRunner.Core.Events.TaskEvents.TaskCrudEvents
{
    /// <summary>
    /// Событие, возникающее при обновлении существующей задачи.
    /// Содержит информацию о том, какие поля были изменены.
    /// </summary>
    public class TaskUpdatedEvent<TID> : ITaskUpdatedEvent<TID>
                                        where TID : IEquatable<TID>
    {
        /// <summary>
        /// Уникальный идентификатор обновлённой задачи
        /// </summary>
        public TID TaskId { get; set; }

        /// <summary>
        /// Наименование обновлённой задачи
        /// </summary>
        public string TaskName { get; set; } = string.Empty;

        /// <summary>
        /// Дата и время обновления задачи в формате UTC
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Массив наименований полей, которые были изменены.
        /// Примеры: "Name", "ExecutionPath", "ScheduleSettings", "IsEnabled"
        /// </summary>
        public string[] ChangedFields { get; set; } = Array.Empty<string>();
    }
}
