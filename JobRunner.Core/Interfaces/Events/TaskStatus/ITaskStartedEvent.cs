using JobRunner.Core.Entities.ValueObjects;
using System;

namespace JobRunner.Core.Interfaces.Events.TaskStatus
{
    /// <summary>
    /// Событие: задача стартовала
    /// </summary>
    public interface ITaskStartedEvent : ITaskEventData, INotifiableEvent
    {
        DateTime StartTime { get; set; }
        long? ProcessId { get; set; }
        string? ExecutionPath { get; set; }
    }
}
