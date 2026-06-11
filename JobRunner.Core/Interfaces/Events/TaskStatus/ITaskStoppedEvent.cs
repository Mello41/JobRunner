using System;

namespace JobRunner.Core.Interfaces.Events.TaskStatus
{
    public interface ITaskStoppedEvent<TID> : ITaskEvent<TID> where TID : IEquatable<TID>
    {
        DateTime StoppedAt { get; set; }
        string StoppedBy { get; set; }
        string Reason { get; set; }
    }
}
