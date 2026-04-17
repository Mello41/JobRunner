using System;

namespace JobRunner.Core.Events
{
    /// <summary>
    /// Аргументы события выполнения задачи (небольшое DTO)
    /// </summary>
    public class JobExecutionEventArgs : EventArgs
    {
        public long TaskId { get; set; }

        public string TaskName { get; set; } = string.Empty;

        public bool Success { get; set; }

        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Время выполнения
        /// </summary>
        public DateTime ExecutionTime { get; set; }

        /// <summary>
        /// Длительность выполнения
        /// </summary>
        public TimeSpan Duration { get; set; }
    }
}
