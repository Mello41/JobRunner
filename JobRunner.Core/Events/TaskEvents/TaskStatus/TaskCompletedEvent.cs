using JobRunner.Core.Entities.ValueObjects;
using JobRunner.Core.Interfaces.Events.TaskStatus;
using System;

namespace JobRunner.Core.Events.TaskEvents.TaskStatus
{
    /// <summary>
    /// Событие, возникающее при завершении выполнения задачи (успешном или с ошибкой).
    /// Содержит информацию о результате выполнения для уведомлений, аудита и анализа.
    /// </summary>
    public class TaskCompletedEvent : ITaskCompletedEvent
    {
        /// <summary>
        /// Уникальный идентификатор завершённой задачи
        /// </summary>
        public Guid TaskId { get; set; }

        /// <summary>
        /// Наименование завершённой задачи
        /// </summary>
        public string TaskName { get; set; } = string.Empty;

        /// <summary>
        /// Флаг успешности выполнения задачи.
        /// true — задача выполнена без ошибок, false — произошла ошибка
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Сообщение об ошибке, если выполнение задачи завершилось неудачно
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Дата и время завершения выполнения задачи в формате UTC
        /// </summary>
        public DateTime CompletionTime { get; set; }

        /// <summary>
        /// Длительность выполнения задачи в миллисекундах
        /// </summary>
        public long? DurationMs { get; set; }

        /// <summary>
        /// Настройки уведомлений задачи, используемые для отправки оповещений о результате выполнения
        /// </summary>
        public INotifySettings NotifySettings { get; set; }
    }
}
