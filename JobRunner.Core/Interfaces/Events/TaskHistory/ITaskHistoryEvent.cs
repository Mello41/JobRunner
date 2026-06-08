using System;

namespace JobRunner.Core.Interfaces.Events.TaskHistory
{
    /// <summary>
    /// Событие истории выполнения задачи.
    /// Содержит полную информацию о каждом запуске задачи для аудита, логирования и аналитики.
    /// </summary>
    /// <typeparam name="TId">Тип идентификатора задачи (Guid, int, long, string)</typeparam>
    public interface ITaskHistoryEvent<TId> : ITaskEventData<TId>
        where TId : IEquatable<TId>
    {
        /// <summary>
        /// Время начала выполнения задачи (UTC)
        /// </summary>
        DateTime StartTime { get; set; }

        /// <summary>
        /// Время окончания выполнения задачи (UTC)
        /// null - задача еще выполняется или не завершилась
        /// </summary>
        DateTime? EndTime { get; set; }

        /// <summary>
        /// Длительность выполнения в миллисекундах
        /// </summary>
        long? DurationMs { get; set; }

        /// <summary>
        /// Флаг успешности выполнения
        /// true - задача завершена без ошибок
        /// false - ошибка, таймаут или отмена
        /// </summary>
        bool Success { get; set; }

        /// <summary>
        /// Статус выполнения задачи
        /// Возможные значения: "completed", "failed", "timeout", "cancelled", "skipped"
        /// </summary>
        string? Status { get; set; }

        /// <summary>
        /// Сообщение об ошибке (если выполнение завершилось с ошибкой)
        /// </summary>
        string? ErrorMessage { get; set; }

        /// <summary>
        /// Идентификатор процесса (PID), в котором выполнялась задача
        /// null - процесс не был запущен или не удалось получить PID
        /// </summary>
        long? ProcessId { get; set; }

        /// <summary>
        /// Код завершения процесса
        /// Интерпретация зависит от конкретного приложения
        /// 0 - обычно успех, другие значения - ошибки
        /// </summary>
        int? ExitCode { get; set; }

        /// <summary>
        /// Источник запуска задачи
        /// Возможные значения: "schedule" (по расписанию), "user" (пользователь), 
        /// "api" (через API), "manual" (ручной запуск)
        /// </summary>
        string? TriggeredBy { get; set; }

        /// <summary>
        /// Стандартный вывод процесса (stdout)
        /// Может быть ограничен по размеру для больших выводов
        /// </summary>
        string? StandardOutput { get; set; }

        /// <summary>
        /// Стандартный вывод ошибок (stderr)
        /// </summary>
        string? StandardError { get; set; }

        /// <summary>
        /// Количество попыток выполнения (включая повторные)
        /// </summary>
        int AttemptNumber { get; set; }

        /// <summary>
        /// Была ли задача выполнена в рамках retry-политики
        /// </summary>
        bool IsRetry { get; set; }
    }
}
