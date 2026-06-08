using System;

namespace JobRunner.Core.Interfaces.Events.TaskCrud
{
    /// <summary>
    /// Событие: задача удалена
    /// </summary>
    public interface ITaskDeletedEvent<TId> : ITaskEventData<TId>
                                            where TId : IEquatable<TId>
    {
        DateTime DeletedAt { get; set; }
        string DeletedBy { get; set; }
    }
}
