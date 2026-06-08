using JobRunner.Core.Entities.ValueObjects;
using JobRunner.Core.Interfaces.Events.TaskStatus;
using System;

namespace JobRunner.Core.Events.TaskEvents.TaskStatus
{
    /// <summary>
    /// Событие, возникающее при начале выполнения задачи.
    /// Содержит информацию о запуске для уведомлений, аудита и мониторинга.
    /// </summary>
    public class TaskStartedEvent<TID> : ITaskStartedEvent<TID>
                                       where TID : IEquatable<TID>
    {
        /// <summary>
        /// Уникальный идентификатор запущенной задачи
        /// </summary>
        public TID TaskId { get; set; }

        /// <summary>
        /// Наименование запущенной задачи
        /// </summary>
        public string TaskName { get; set; } = string.Empty;

        /// <summary>
        /// Дата и время начала выполнения задачи в формате UTC
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Идентификатор процесса (PID), в котором выполняется задача.
        /// Может быть null в случае ошибки запуска или асинхронного режима без ожидания
        /// </summary>
        public long? ProcessId { get; set; }

        /// <summary>
        /// Полный путь к исполняемому файлу задачи
        /// </summary>
        public string? ExecutionPath { get; set; }

        /// <summary>
        /// Настройки уведомлений задачи, используемые для отправки оповещений о начале выполнения
        /// </summary>
        public INotifySettings NotifySettings { get; set; }
    }
}
