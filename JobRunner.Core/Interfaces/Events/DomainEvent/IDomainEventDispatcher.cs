using System.Threading;
using System.Threading.Tasks;

namespace JobRunner.Core.Interfaces.Events.DomainEvent
{
    /// <summary>
    /// Диспетчер доменных событий
    /// </summary>
    public interface IDomainEventDispatcher
    {
        /// <summary>
        /// Публикует событие всем зарегистрированным обработчикам
        /// </summary>
        /// <typeparam name="TEvent">Тип события (должен быть классом)</typeparam>
        /// <param name="event">Событие для публикации</param>
        /// <param name="cancellationToken">Токен отмены</param>
        Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : class;
    }
}
