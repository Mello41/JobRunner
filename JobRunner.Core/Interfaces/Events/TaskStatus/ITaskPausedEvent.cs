using System;

namespace JobRunner.Core.Interfaces.Events.TaskStatus
{
    public interface ITaskPausedEvent<TID> : ITaskEvent<TID> where TID : IEquatable<TID>
    {
        DateTime PausedAt { get; set; }
        string Reason { get; set; }
    }
}
