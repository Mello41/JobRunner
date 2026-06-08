using System;

namespace JobRunner.Core.Interfaces.Events.TaskStatus
{
    /// <summary>
    /// Событие: задача стартовала
    /// </summary>
    public interface ITaskStartedEvent<TId> : ITaskEventData<TId>, INotifiableEvent
                                            where TId : IEquatable<TId>
    {
        DateTime StartTime { get; set; }
        long? ProcessId { get; set; }
        string? ExecutionPath { get; set; }
    }
}
