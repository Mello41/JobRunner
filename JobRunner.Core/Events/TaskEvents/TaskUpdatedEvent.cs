using JobRunner.Core.Interfaces.Events;
using System;

namespace JobRunner.Core.Events.TaskEvents
{
    public class TaskUpdatedEvent : ITaskUpdatedEvent
    {
        public Guid TaskId { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
        public string[] ChangedFields { get; set; } = Array.Empty<string>();
    }
}
