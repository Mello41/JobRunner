using JobRunner.Core.Interfaces.Entities.JobTaskSettings;
using JobRunner.Core.Interfaces.Entities.Retry;
using System;

namespace JobRunner.Core.DefaultImplementations.Strategies
{
    /// <summary>
    /// Линейная задержка (attempt * initial)
    /// </summary>
    public class IncrementalStrategy : IRetryStrategy
    {
        public string Name => "Incremental";

        public int GetDelayMilliseconds(int attemptNumber, IRetrySettings settings)
        {
            if (attemptNumber <= 1) return 0;

            var delaySeconds = settings.InitialDelaySeconds * attemptNumber;

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
