using System;

namespace JobRunner.Core.Events
{
    /// <summary>
    /// Событие: задача завершила выполнение
    /// </summary>
    public class TaskCompletedEvent
    {
        public Guid TaskId { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime CompletionTime { get; set; }
        public long DurationMs { get; set; }
    }
}
