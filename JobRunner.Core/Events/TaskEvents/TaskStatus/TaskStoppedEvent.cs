using JobRunner.Core.Interfaces.Entities.JobTaskSettings;
using System;
using JobRunner.Core.Interfaces.Events.TaskStatus;

namespace JobRunner.Core.Events.TaskEvents.TaskStatus
{
    /// <summary>
    /// Событие, возникающее при принудительной остановке задачи
    /// </summary>
    public class TaskStoppedEvent<TID> : ITaskStoppedEvent<TID>
        where TID : IEquatable<TID>
    {
        public TID TaskId { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public DateTime StoppedAt { get; set; }
        public string StoppedBy { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public INotifySettings NotifySettings { get; set; }
    }
}
