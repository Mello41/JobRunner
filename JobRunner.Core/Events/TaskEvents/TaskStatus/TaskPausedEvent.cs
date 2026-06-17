using System;
using JobRunner.Core.Interfaces.Events.TaskStatus;
using JobRunner.Core.Interfaces.Entities.JobTaskSettings.NotifySettings;

namespace JobRunner.Core.Events.TaskEvents.TaskStatus
{
    /// <summary>
    /// Событие, возникающее при приостановке выполнения задачи
    /// </summary>
    public class TaskPausedEvent<TID> : ITaskPausedEvent<TID>
        where TID : IEquatable<TID>
    {
        public TID TaskId { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public DateTime PausedAt { get; set; }
        public string Reason { get; set; } = string.Empty;
        public INotifySettings NotifySettings { get; set; }
    }
}
