using System;

namespace JobRunner.Core.Interfaces.Events.TaskCrud
{
    /// <summary>
    /// Событие: задача создана
    /// </summary>
    public interface ITaskCreatedEvent<TId> : ITaskEventData<TId>
                                            where TId : IEquatable<TId>
    {
        DateTime CreatedAt { get; set; }
        bool IsEnabled { get; set; }
        string? ScheduleDescription { get; set; }
    }
}
