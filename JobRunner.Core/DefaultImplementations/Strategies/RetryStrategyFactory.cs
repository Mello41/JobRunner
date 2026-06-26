using JobRunner.Core.Entities.Enums;
using JobRunner.Core.Interfaces.Entities.Retry;

namespace JobRunner.Core.DefaultImplementations.Strategies
{
    /// <summary>
    /// Фабрика для создания стратегий на основе enum
    /// </summary>
    public static class RetryStrategyFactory
    {
        public static IRetryPolicy Create(RetryStrategyTypes strategyType)
        {
            return strategyType switch
            {
                RetryStrategyTypes.None => new NoRetryStrategy(),
                RetryStrategyTypes.FixedDelay => new FixedDelayStrategy(),
                RetryStrategyTypes.ExponentialBackoff => new ExponentialBackoffStrategy(),
                RetryStrategyTypes.Incremental => new IncrementalStrategy(),
                _ => new FixedDelayStrategy()
            };
        }
    }
}
