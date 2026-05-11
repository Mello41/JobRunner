using JobRunner.Core.Entities.Enums;
using System;

namespace JobRunner.Core.DTO
{
    /// <summary>
    /// DTO для уведомления UI
    /// </summary>
    public class UiNotification
    {
        /// <summary>
        /// Тип события (используйте EventType enum)
        /// </summary>
        public EventType TypeNotification { get; set; }

        /// <summary>
        /// ID задачи
        /// </summary>
        public Guid TaskId { get; set; }

        /// <summary>
        /// Название задачи
        /// </summary>
        public string TaskName { get; set; } = string.Empty;

        /// <summary>
        /// Человекочитаемое сообщение
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Время события (UTC)
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Дополнительные данные (статус, ошибка, PID и т.д.)
        /// </summary>
        public object? Data { get; set; }

        /// <summary>
        /// Уровень важности для UI (опционально)
        /// </summary>
        public NotificationSeverity Severity { get; set; } = NotificationSeverity.Info;

        /// <summary>
        /// Создать уведомление с сообщением по умолчанию
        /// </summary>
        public static UiNotification FromEvent(EventType eventType, Guid taskId, string taskName, object? data = null)
        {
            var message = GetDefaultMessage(eventType, taskName);
            var severity = GetSeverity(eventType);

            return new UiNotification
            {
                TypeNotification = eventType,
                TaskId = taskId,
                TaskName = taskName,
                Message = message,
                Data = data,
                Severity = severity,
                Timestamp = DateTime.UtcNow
            };
        }

        private static string GetDefaultMessage(EventType eventType, string taskName)
        {
            return eventType switch
            {
                EventType.TaskCreated => $"Задача '{taskName}' создана",
                EventType.TaskUpdated => $"Задача '{taskName}' обновлена",
                EventType.TaskDeleted => $"Задача '{taskName}' удалена",
                EventType.TaskStarted => $"Задача '{taskName}' запущена",
                EventType.TaskCompleted => $"Задача '{taskName}' завершена",
                EventType.TaskPaused => $"Задача '{taskName}' приостановлена",
                EventType.TaskResumed => $"Задача '{taskName}' возобновлена",
                EventType.TaskStopped => $"Задача '{taskName}' остановлена",
                EventType.TaskSkipped => $"Задача '{taskName}' пропущена",
                EventType.TaskFailed => $"Задача '{taskName}' завершилась с ошибкой",
                EventType.TaskTimedOut => $"Задача '{taskName}' превысила таймаут",
                _ => $"Событие {eventType} для задачи '{taskName}'"
            };
        }

        private static NotificationSeverity GetSeverity(EventType eventType)
        {
            return eventType switch
            {
                EventType.TaskCreated => NotificationSeverity.Success,
                EventType.TaskDeleted => NotificationSeverity.Warning,
                EventType.TaskStarted => NotificationSeverity.Info,
                EventType.TaskCompleted => NotificationSeverity.Success,
                EventType.TaskFailed => NotificationSeverity.Error,
                EventType.TaskTimedOut => NotificationSeverity.Error,
                EventType.TaskSkipped => NotificationSeverity.Warning,
                _ => NotificationSeverity.Info
            };
        }
    }
}