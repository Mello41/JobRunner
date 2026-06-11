using JobRunner.Core.Interfaces.Entities.JobTaskSettings;
using System;
using System.Linq;

namespace JobRunner.Core.DefaultImplementations.Strategies
{
    /// <summary>
    /// Базовый класс с общей логикой ShouldRetry
    /// </summary>
    internal static class RetryStrategyHelpers
    {
        public static bool ShouldRetryCore(string? errorMessage, int? exitCode, bool isTimeout,
                                           int currentAttempt, IRetrySettings settings)
        {
            if (currentAttempt >= settings.MaxAttempts)
                return false;

            if (isTimeout)
                return settings.RetryOnTimeout;

            if (settings.RetryOnAnyError)
                return true;

            if (exitCode.HasValue && settings.RetryableExitCodes.Contains(exitCode.Value))
                return true;

            if (!string.IsNullOrEmpty(errorMessage) && settings.RetryableErrorMessages.Any())
            {
                return settings.RetryableErrorMessages.Any(msg =>
                    errorMessage.IndexOf(msg, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            return false;
        }
    }
}
