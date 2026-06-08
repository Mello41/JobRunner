using System;

namespace JobRunner.Core.Interfaces.Events.TaskCrud
{
    /// <summary>
    /// Событие: задача обновлена
    /// </summary>
    public interface ITaskUpdatedEvent<TId> : ITaskEventData<TId>
                                            where TId : IEquatable<TId>
    {
        DateTime UpdatedAt { get; set; }
        string[] ChangedFields { get; set; }
    }
}
