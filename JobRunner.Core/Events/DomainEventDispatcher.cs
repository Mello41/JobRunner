using System;
using System.Threading;
using System.Threading.Tasks;
using JobRunner.Core.Interfaces.Events.DomainEvent;
using Microsoft.Extensions.DependencyInjection;

namespace JobRunner.Core.Events
{
    /// <summary>
    /// Реализация диспетчера доменных событий
    /// </summary>
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public DomainEventDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task PublishAsync<TEvent>(TEvent @evt, CancellationToken cancellationToken = default)
            where TEvent : class
        {
            var handlers = _serviceProvider.GetServices<IDomainEventHandler<TEvent>>();

            foreach (var handler in handlers)
            {
                await handler.HandleAsync(@evt, cancellationToken);
            }
        }
    }
}
