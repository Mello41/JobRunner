using System;

namespace JobRunner.Core.Interfaces.Events.TaskStatus
{
    public interface ITaskFailedEvent<TID> : ITaskEvent<TID> where TID : IEquatable<TID>
    {
        string ErrorMessage { get; set; }
        DateTime FailureTime { get; set; }
        string StackTrace { get; set; }
    }
}
