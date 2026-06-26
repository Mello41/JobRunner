using JobRunner.Core.Interfaces.Entities.JobTaskSettings;
using JobRunner.Core.Interfaces.Entities.Retry;
using System;

namespace JobRunner.Core.DefaultImplementations.Strategies
{
    /// <summary>
    /// Стратегия с экспоненциальной задержкой
    /// </summary>
    public class ExponentialBackoffStrategy : IRetryPolicy
    {
        public string Name => "Exponential backoff";

        public int GetDelayMilliseconds(int attemptNumber, IRetrySettings settings)
        {
            if (attemptNumber <= 1) return 0;

            var delaySeconds = settings.InitialDelaySeconds * Math.Pow(2, attemptNumber - 1);

            if (settings.MaxDelaySeconds.HasValue)
                delaySeconds = Math.Min(delaySeconds, settings.MaxDelaySeconds.Value);

            return (int)delaySeconds * 1000;
        }

        public bool ShouldRetry(string? errorMessage, int? exitCode, bool isTimeout,
                                int currentAttempt, IRetrySettings settings)
        {
            return RetryStrategyHelpers.ShouldRetryCore(errorMessage, exitCode, isTimeout, currentAttempt, settings);
        }
    }
}
