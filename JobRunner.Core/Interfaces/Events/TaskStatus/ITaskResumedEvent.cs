using System;

namespace JobRunner.Core.Interfaces.Events.TaskStatus
{
    public interface ITaskResumedEvent<TID> : ITaskEvent<TID> where TID : IEquatable<TID>
    {
        DateTime ResumedAt { get; set; }
    }
}
