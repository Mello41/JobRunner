using System;

namespace JobRunner.Core.Interfaces.Events.TaskStatus
{
    public interface ITaskTimedOutEvent<TID> : ITaskEvent<TID> where TID : IEquatable<TID>
    {
        DateTime TimeoutAt { get; set; }
        int TimeoutSeconds { get; set; }
        long ElapsedMs { get; set; }
    }
}
