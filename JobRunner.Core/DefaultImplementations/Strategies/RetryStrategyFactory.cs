using JobRunner.Core.Entities.Enums;
using JobRunner.Core.Interfaces.Entities.Retry;

namespace JobRunner.Core.DefaultImplementations.Strategies
{
    /// <summary>
    /// Фабрика для создания стратегий на основе enum
    /// </summary>
    public static class RetryStrategyFactory
    {
        public static IRetryStrategy Create(RetryStrategy strategyType)
        {
            return strategyType switch
            {
                RetryStrategy.None => new NoRetryStrategy(),
                RetryStrategy.FixedDelay => new FixedDelayStrategy(),
                RetryStrategy.ExponentialBackoff => new ExponentialBackoffStrategy(),
                RetryStrategy.Incremental => new IncrementalStrategy(),
                _ => new FixedDelayStrategy()
            };
        }
    }
}
