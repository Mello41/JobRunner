using System;
using JobRunner.Core.Interfaces.Events.TaskStatus;
using JobRunner.Core.Interfaces.Entities.JobTaskSettings.NotifySettings;

namespace JobRunner.Core.Events.TaskEvents.TaskStatus
{
    /// <summary>
    /// Событие, возникающее при возобновлении выполнения задачи
    /// </summary>
    public class TaskResumedEvent<TID> : ITaskResumedEvent<TID>
                                    where TID : IEquatable<TID>
    {
        public TID TaskId { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public DateTime ResumedAt { get; set; }
        public INotifySettings<TID> NotifySettings { get; set; }
    }
}
