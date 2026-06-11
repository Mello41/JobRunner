using System;

namespace JobRunner.Core.Interfaces.Events.TaskStatus
{
    public interface ITaskSkippedEvent<TID> : ITaskEvent<TID> where TID : IEquatable<TID>
    {
        DateTime SkippedAt { get; set; }
        string Reason { get; set; }
    }
}
