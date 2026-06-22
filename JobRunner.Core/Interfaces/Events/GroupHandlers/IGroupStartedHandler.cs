using JobRunner.Core.Interfaces.Events.DomainEvent;
using JobRunner.Core.Interfaces.Events.GroupEvents;
using System;

namespace JobRunner.Core.Interfaces.Events.GroupHandlers
{
    /// <summary>
    /// Обработчик события запуска группы
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public interface IGroupStartedHandler<TId> : IDomainEventHandler<IGroupStartedEvent<TId>>
        where TId : IEquatable<TId>
    {

    }
}
