using System.Threading;
using System.Threading.Tasks;

namespace JobRunner.Core.Interfaces.Events.DomainEvent
{
    /// <summary>
    /// Интерфейс обработчика доменных событий
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public interface IDomainEventHandler<in TEvent> where TEvent : class
    {
        /// <summary>
        /// Обрабатывает событие
        /// </summary>
        /// <param name="event">Событие для обработки</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns></returns>
        Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
    }
}
