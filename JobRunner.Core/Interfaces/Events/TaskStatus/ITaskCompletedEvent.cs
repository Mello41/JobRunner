using System;

namespace JobRunner.Core.Interfaces.Events.TaskStatus
{
    /// <summary>
    /// Событие: задача завершена
    /// </summary>
    public interface ITaskCompletedEvent : ITaskEventData
    {
        bool Success { get; set; }
        string? ErrorMessage { get; set; }
        DateTime CompletionTime { get; set; }
        long DurationMs { get; set; }
    }
}
