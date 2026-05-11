using System;

namespace JobRunner.Core.Interfaces.Events
{
    /// <summary>
    /// Событие: задача обновлена
    /// </summary>
    public interface ITaskUpdatedEvent : ITaskEventData
    {
        DateTime UpdatedAt { get; set; }
        string[] ChangedFields { get; set; }
    }
}
