using FluentAssertions;
using JobRunner.Core.Events;

namespace JobRunner.Domain.Tests.CoreTests.DomainEvent
{
    /*
    public class DomainEventDispatcherTests
    {
        [Fact]
        public async Task PublishAsync_WhenOneHandlerThrows_OtherHandlersShouldStillExecute()
        {
            // КРИТИЧЕСКИ ВАЖНО: ошибка в одном обработчике не должна ломать другие
            var dispatcher = new DomainEventDispatcher(serviceProvider);

            var handler1 = new ThrowingHandler(); // выбрасывает исключение
            var handler2 = new SuccessHandler();   // работает нормально

            await dispatcher.PublishAsync(new TestEvent());

            Assert.True(handler2.WasCalled);
        }

        [Fact]
        public async Task PublishAsync_WhenNoHandlers_ShouldNotThrow()
        {
            var dispatcher = new DomainEventDispatcher(serviceProvider);

            var act = async () => await dispatcher.PublishAsync(new TestEvent());

            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task PublishAsync_ShouldCallAllHandlersEvenIfSomeAreAsyncSlow()
        {
            // Проверка, что все обработчики получают событие
            // даже если один из них медленный
        }
    */
    }
