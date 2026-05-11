using JobRunner.Core.Entities.ValueObjects;
using JobRunner.Core.Interfaces.Events.TaskStatus;
using System;

namespace JobRunner.Core.Events.TaskEvents.TaskStatus
{
    /// <summary>
    /// Событие: задача начала выполнение
    /// </summary>
    public class TaskStartedEvent : ITaskStartedEvent
    {
        public Guid TaskId { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public long? ProcessId { get; set; }
        public string? ExecutionPath { get; set; }
        public INotifySettings NotifySettings { get; set; }
    }
}
