using JobRunner.Core.Entities.ValueObjects;
using JobRunner.Core.Interfaces.Events.TaskStatus;
using System;

namespace JobRunner.Core.Events.TaskEvents.TaskStatus
{
    /// <summary>
    /// Событие: задача завершила выполнение
    /// </summary>
    public class TaskCompletedEvent : ITaskCompletedEvent
    {
        public Guid TaskId { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime CompletionTime { get; set; }
        public long? DurationMs { get; set; }
        public INotifySettings NotifySettings { get; set; }
    }
}
