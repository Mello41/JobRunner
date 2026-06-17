using System;
using JobRunner.Core.Interfaces.Events.TaskStatus;
using JobRunner.Core.Interfaces.Entities.JobTaskSettings.NotifySettings;

namespace JobRunner.Core.Events.TaskEvents.TaskStatus
{
    /// <summary>
    /// Событие, возникающее при превышении таймаута выполнения задачи
    /// </summary>
    public class TaskTimedOutEvent<TID> : ITaskTimedOutEvent<TID>
        where TID : IEquatable<TID>
    {
        public TID TaskId { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public DateTime TimeoutAt { get; set; }
        public int TimeoutSeconds { get; set; }
        public long ElapsedMs { get; set; }
        public INotifySettings NotifySettings { get; set; }
    }
}
