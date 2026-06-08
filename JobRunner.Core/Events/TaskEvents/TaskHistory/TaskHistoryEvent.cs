using JobRunner.Core.Interfaces.Events.TaskHistory;
using JobRunner.Core.Results;
using System;

namespace JobRunner.Core.Events.TaskEvents.TaskHistory
{
    /// <summary>
    /// Событие, возникающее при каждом выполнении задачи, содержащее полную информацию о запуске и результате.
    /// Используется для аудита, логирования, сбора метрик и анализа производительности.
    /// </summary>
    public class TaskHistoryEvent<TId> : ITaskHistoryEvent<TId>
        where TId : IEquatable<TId>
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
        /// Время окончания выполнения задачи в формате UTC
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// Длительность выполнения задачи в миллисекундах
        /// </summary>
        public long? DurationMs { get; set; }

        /// <summary>
        /// Флаг успешности выполнения задачи
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Статус выполнения задачи
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// Сообщение об ошибке, если выполнение задачи завершилось с ошибкой
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Идентификатор процесса (PID), в котором выполнялась задача
        /// </summary>
        public long? ProcessId { get; set; }

        /// <summary>
        /// Код завершения процесса, возвращённый при его остановке
        /// </summary>
        public int? ExitCode { get; set; }

        /// <summary>
        /// Источник запуска задачи
        /// </summary>
        public string? TriggeredBy { get; set; }

        /// <summary>
        /// Стандартный вывод процесса (stdout)
        /// </summary>
        public string? StandardOutput { get; set; }

        /// <summary>
        /// Стандартный вывод ошибок (stderr)
        /// </summary>
        public string? StandardError { get; set; }

        /// <summary>
        /// Количество попыток выполнения
        /// </summary>
        public int AttemptNumber { get; set; }

        /// <summary>
        /// Была ли задача выполнена в рамках retry-политики
        /// </summary>
        public bool IsRetry { get; set; }

        /// <summary>
        /// Создает событие из результата выполнения
        /// </summary>
        public static TaskHistoryEvent<TId> FromExecutionResult(
            TId taskId,
            string taskName,
            DateTime startTime,
            JobExecutionResult result,
            string? triggeredBy = "schedule",
            int attemptNumber = 1,
            bool isRetry = false)
        {
            return new TaskHistoryEvent<TId>
            {
                TaskId = taskId,
                TaskName = taskName,
                StartTime = startTime,
                EndTime = result.EndTime ?? DateTime.UtcNow,
                DurationMs = result.DurationMs,
                Success = result.Success,
                Status = result.IsTimeout ? "timeout" : (result.Success ? "completed" : "failed"),
                ErrorMessage = result.ErrorMessage,
                ProcessId = result.ProcessId,
                ExitCode = result.ExitCode,
                TriggeredBy = triggeredBy,
                StandardOutput = result.StandardOutput,
                StandardError = result.StandardError,
                AttemptNumber = attemptNumber,
                IsRetry = isRetry
            };
        }
    }
}
