using System;

namespace JobRunner.Core.Entities.ValueObjects
{
    /// <summary>
    /// Метаданные выполнения задачи (статистика, история, состояние)
    /// </summary>
    public interface IJobTaskMetadata
    {
        /// <summary>
        /// PID текущего процесса (если запущена)
        /// </summary>
        long? TaskPID { get; set; }

        /// <summary>
        /// Дата последнего запуска
        /// </summary>
        DateTime? LastRun { get; set; }

        /// <summary>
        /// Дата следующего запланированного запуска
        /// </summary>
        DateTime? NextRun { get; set; }

        /// <summary>
        /// Длительность последнего успешного выполнения (в миллисекундах)
        /// </summary>
        long? LastDurationMs { get; set; }

        /// <summary>
        /// Общее количество запусков задачи за всё время
        /// </summary>
        int TotalRunCount { get; set; }

        /// <summary>
        /// Количество успешных запусков
        /// </summary>
        int SuccessCount { get; set; }

        /// <summary>
        /// Количество неудачных запусков
        /// </summary>
        int FailureCount { get; set; }

        /// <summary>
        /// Версия
        /// </summary>
        string Version { get; set; }

        /// <summary>
        /// Количество последовательных ошибок подряд (для уведомлений и отказоустойчивости)
        /// </summary>
        int ConsecutiveFailures { get; set; }

        /// <summary>
        /// Последняя ошибка (сообщение)
        /// </summary>
        string? LastError { get; set; }

        /// <summary>
        /// Время последней ошибки
        /// </summary>
        DateTime? LastErrorTime { get; set; }

        /// <summary>
        /// Задача запущена в данный момент?
        /// </summary>
        bool IsRunning { get; set; }

        /// <summary>
        /// Задача завершена (для однократных задач)
        /// </summary>
        bool IsCompleted { get; set; }

        /// <summary>
        /// Сбросить статистику (на всякий случай)
        /// </summary>
        void ResetStats();
    }
}
