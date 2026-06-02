using System;

namespace JobRunner.Core.Results
{
    /// <summary>
    /// Результат выполнения задачи
    /// </summary>
    public class JobExecutionResult
    {
        /// <summary>
        /// Успешно ли выполнена задача
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// PID процесса (если запущен)
        /// </summary>
        public long? ProcessId { get; set; }

        /// <summary>
        /// Сообщение об ошибке (если есть)
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Время начала выполнения
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Время окончания выполнения (если завершена)
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// Длительность выполнения в миллисекундах
        /// </summary>
        public long? DurationMs { get; set; }

        /// <summary>
        /// Был ли превышен таймаут выполнения
        /// </summary>
        public bool IsTimeout { get; set; }

        /// <summary>
        /// Выходной код процесса (если есть)
        /// </summary>
        public int? ExitCode { get; set; }

        /// <summary>
        /// Стандартный вывод процесса
        /// </summary>
        public string? StandardOutput { get; set; }

        /// <summary>
        /// Стандартный вывод ошибок
        /// </summary>
        public string? StandardError { get; set; }

        /// <summary>
        /// Создать успешный результат
        /// </summary>
        public static JobExecutionResult CreateSuccess(long? processId, DateTime startTime, DateTime? endTime = null, long? durationMs = null)
        {
            return new JobExecutionResult
            {
                Success = true,
                ProcessId = processId,
                StartTime = startTime,
                EndTime = endTime,
                DurationMs = durationMs
            };
        }

        /// <summary>
        /// Создать результат с ошибкой
        /// </summary>
        public static JobExecutionResult CreateFailure(string errorMessage, DateTime startTime, int? exitCode = null)
        {
            return new JobExecutionResult
            {
                Success = false,
                ErrorMessage = errorMessage,
                StartTime = startTime,
                EndTime = DateTime.UtcNow,
                ExitCode = exitCode
            };
        }

        /// <summary>
        /// Создать результат с таймаутом
        /// </summary>
        public static JobExecutionResult CreateTimeout(int timeoutSeconds, DateTime startTime)
        {
            var endTime = DateTime.UtcNow;
            return new JobExecutionResult
            {
                Success = false,
                ErrorMessage = $"Task exceeded timeout of {timeoutSeconds} seconds",
                StartTime = startTime,
                EndTime = endTime,
                DurationMs = (long)(endTime - startTime).TotalMilliseconds,
                IsTimeout = true
            };
        }
    }
}
