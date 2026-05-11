using JobRunner.Core.Interfaces.Events.TaskCrud;
using System;

namespace JobRunner.Core.Events.TaskEvents.TaskCrudEvents
{
    public class TaskDeletedEvent : ITaskDeletedEvent
    {
        public Guid TaskId { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public DateTime DeletedAt { get; set; }
        public string DeletedBy { get; set; } = "system";
    }
}
