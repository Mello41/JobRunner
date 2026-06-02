using JobRunner.Core.Entities.ValueObjects;

namespace JobRunner.Core.DefaultImplementations.Strategies
{
    /// <summary>
    /// Без повторных попыток
    /// </summary>
    public class NoRetryStrategy : IRetryStrategy
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
