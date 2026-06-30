using JobRunner.Core.Interfaces.Entities.JobTaskSettings;
using JobRunner.Core.Interfaces.Entities.Retry;

namespace JobRunner.Core.DefaultImplementations.Strategies
{
    /// <summary>
    /// Без повторных попыток
    /// </summary>
    public class NoRetryStrategy : IRetryPolicyStrategy
    {
        public string Name => "No retry";

        public int GetDelayMilliseconds(int attemptNumber, IRetrySettings settings)
        {
            return 0;
        }

        public bool ShouldRetry(string? errorMessage, int? exitCode, bool isTimeout,
                                int currentAttempt, IRetrySettings settings)
        {
            return false;
        }
    }
}
