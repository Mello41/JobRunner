using JobRunner.Core.Interfaces.Events.TaskCrud;
using System;

namespace JobRunner.Core.Events.TaskEvents.TaskCrudEvents
{
    /// <summary>
    /// Событие, возникающее при удалении задачи из системы.
    /// Содержит информацию об удалённой задаче для аудита и логирования.
    /// </summary>
    public class TaskDeletedEvent : ITaskDeletedEvent
    {
        /// <summary>
        /// Уникальный идентификатор удалённой задачи
        /// </summary>
        public Guid TaskId { get; set; }

        /// <summary>
        /// Наименование удалённой задачи
        /// </summary>
        public string TaskName { get; set; } = string.Empty;

        /// <summary>
        /// Дата и время удаления задачи в формате UTC
        /// </summary>
        public DateTime DeletedAt { get; set; }

        /// <summary>
        /// Идентификатор или наименование инициатора удаления.
        /// По умолчанию "system", может содержать имя пользователя или идентификатор API-ключа
        /// </summary>
        public string DeletedBy { get; set; } = "system";
    }
}