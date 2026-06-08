using JobRunner.Core.Interfaces.Events.TaskCrud;
using System;

namespace JobRunner.Core.Events.TaskEvents.TaskCrudEvents
{
    /// <summary>
    /// Событие: создана задача
    /// </summary>
    public class TaskCreatedEvent<TId> : ITaskCreatedEvent<TId>
                                        where TId : IEquatable<TId>
    {
        /// <summary>
        /// Уникальный идентификатор задачи
        /// </summary>
        public TId TaskId { get; set; }

        /// <summary>
        /// Наименование задачи
        /// </summary>
        public string TaskName { get; set; } = string.Empty;

        /// <summary>
        /// Дата и время создания задачи (UTC)
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Активна ли задача (включена/выключена)
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Человекочитаемое описание расписания (например "Ежедневно в 14:30")
        /// </summary>
        public string? ScheduleDescription { get; set; }
    }
}