using JobRunner.Core.DefaultImplementations.Strategies;
using JobRunner.Core.Interfaces.Entities.JobTaskSettings;
using JobRunner.Core.Interfaces.Entities.Retry;

namespace JobRunner.Core.DefaultImplementations.Strategies
{
    /// <summary>
    /// Фиксированная задержка
    /// </summary>
    public class FixedDelayStrategy : IRetryStrategy
    {
        public string Name => "Fixed delay";

        public int GetDelayMilliseconds(int attemptNumber, IRetrySettings settings)
        {
            if (attemptNumber <= 1) return 0;
            return settings.InitialDelaySeconds * 1000;
        }

        public bool ShouldRetry(string? errorMessage, int? exitCode, bool isTimeout,
                                int currentAttempt, IRetrySettings settings)
        {
            return RetryStrategyHelpers.ShouldRetryCore(errorMessage, exitCode, isTimeout, currentAttempt, settings);
        }
    }
}
