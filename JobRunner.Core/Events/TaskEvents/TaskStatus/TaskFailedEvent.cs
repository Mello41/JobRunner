using JobRunner.Core.Interfaces.Entities.JobTaskSettings.NotifySettings;
using JobRunner.Core.Interfaces.Events.TaskStatus;
using System;

namespace JobRunner.Core.Events.TaskEvents.TaskStatus
{
    /// <summary>
    /// Событие, возникающее при ошибке выполнения задачи
    /// </summary>
    public class TaskFailedEvent<TID> : ITaskFailedEvent<TID>
        where TID : IEquatable<TID>
    {
        public TID TaskId { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public DateTime FailureTime { get; set; }
        public string StackTrace { get; set; } = string.Empty;
        public INotifySettings NotifySettings { get; set; }
    }
}
