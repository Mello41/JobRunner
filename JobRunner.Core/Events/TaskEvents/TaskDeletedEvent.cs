using JobRunner.Core.Interfaces.Events;
using System;

namespace JobRunner.Core.Events.TaskEvents
{
    public class TaskDeletedEvent : ITaskDeletedEvent
    {
        public Guid TaskId { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public DateTime DeletedAt { get; set; }
        public string DeletedBy { get; set; } = "system";
    }
}
