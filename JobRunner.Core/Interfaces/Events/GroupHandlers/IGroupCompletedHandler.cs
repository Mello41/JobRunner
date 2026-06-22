using JobRunner.Core.Interfaces.Events.DomainEvent;
using JobRunner.Core.Interfaces.Events.GroupEvents;
using System;

namespace JobRunner.Core.Interfaces.Events.GroupHandlers
{
    /// <summary>
    /// Обработчик события завершения группы
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public interface IGroupCompletedHandler<TId> : IDomainEventHandler<IGroupCompletedEvent<TId>>
        where TId : IEquatable<TId>
    {

    }
}
