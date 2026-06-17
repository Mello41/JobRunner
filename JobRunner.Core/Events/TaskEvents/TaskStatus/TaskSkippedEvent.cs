using System;
using JobRunner.Core.Interfaces.Events.TaskStatus;
using JobRunner.Core.Interfaces.Entities.JobTaskSettings.NotifySettings;

namespace JobRunner.Core.Events.TaskEvents.TaskStatus
{
    /// <summary>
    /// Событие, возникающее при пропуске выполнения задачи
    /// </summary>
    public class TaskSkippedEvent<TID> : ITaskSkippedEvent<TID>
        where TID : IEquatable<TID>
    {
        public TID TaskId { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public DateTime SkippedAt { get; set; }
        public string Reason { get; set; } = string.Empty;
        public INotifySettings<TID> NotifySettings { get; set; }
    }
}
