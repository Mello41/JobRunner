using System;

namespace JobRunner.Core.Events.TaskEvents.TaskHistory
{
    /// <summary>
    /// Событие, возникающее при каждом выполнении задачи, содержащее полную информацию о запуске и результате.
    /// Используется для аудита, логирования, сбора метрик и анализа производительности.
    /// Подписчики могут сохранять эти данные в БД, отправлять в системы мониторинга или уведомления.
    /// </summary>
    public class TaskHistoryEvent<TId> where TId : IEquatable<TId>
    {
        /// <summary>
        /// Уникальный идентификатор задачи
        /// </summary>
        public TId TaskId { get; set; }

        /// <summary>
        /// Наименование задачи (снэпшот на момент выполнения)
        /// </summary>
        public string TaskName { get; set; } = string.Empty;

        /// <summary>
        /// Время начала выполнения задачи в формате UTC
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Время окончания выполнения задачи в формате UTC.
        /// Значение null указывает на то, что задача ещё выполняется или не завершена
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// Длительность выполнения задачи в миллисекундах
        /// </summary>
        public long? DurationMs { get; set; }

        /// <summary>
        /// Флаг успешности выполнения задачи.
        /// true — задача завершена без ошибок, false — с ошибкой, таймаутом или отменой
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Статус выполнения задачи.
        /// Возможные значения: "completed", "failed", "timeout", "cancelled", "skipped"
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// Сообщение об ошибке, если выполнение задачи завершилось с ошибкой
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Идентификатор процесса (PID), в котором выполнялась задача.
        /// Значение null, если процесс не был запущен или не удалось получить PID
        /// </summary>
        public long? ProcessId { get; set; }

        /// <summary>
        /// Код завершения процесса, возвращённый при его остановке.
        /// Интерпретация кода зависит от конкретного приложения
        /// </summary>
        public int? ExitCode { get; set; }

        /// <summary>
        /// Источник запуска задачи.
        /// Возможные значения: "schedule" (по расписанию), "user" (пользователь), "api" (через API)
        /// </summary>
        public string? TriggeredBy { get; set; }
    }
}
