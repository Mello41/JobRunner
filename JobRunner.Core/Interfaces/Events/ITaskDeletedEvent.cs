using System;

namespace JobRunner.Core.Interfaces.Events
{
    /// <summary>
    /// Событие: задача удалена
    /// </summary>
    public interface ITaskDeletedEvent : ITaskEventData
    {
        DateTime DeletedAt { get; set; }
        string DeletedBy { get; set; }
    }
}
