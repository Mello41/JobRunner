using System;

namespace JobRunner.Core.Interfaces.Events
{
    /// <summary>
    /// Событие: задача создана
    /// </summary>
    public interface ITaskCreatedEvent : ITaskEventData
    {
        DateTime CreatedAt { get; set; }
        bool IsEnabled { get; set; }
        string? ScheduleDescription { get; set; }
    }
}
