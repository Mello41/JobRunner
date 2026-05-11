using JobRunner.Core.Interfaces.Events;
using System;

namespace JobRunner.Core.Events.TaskEvents
{
    public class TaskCreatedEvent : ITaskCreatedEvent
    {
        public Guid TaskId { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsEnabled { get; set; }
        public string? ScheduleDescription { get; set; }
    }
}
