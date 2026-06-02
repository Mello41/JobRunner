using JobRunner.Core.DefaultImplementations.Strategies;
using JobRunner.Core.Entities.Enums;
using JobRunner.Core.Entities.ValueObjects;
using System.Collections.Generic;

namespace JobRunner.Core.DefaultImplementations
{
    public class RetrySettings : IRetrySettings
    {
        private readonly List<string> _retryableErrorMessages = new();
        private readonly List<int> _retryableExitCodes = new();

        public int MaxAttempts { get; set; } = 1;

        private IRetryStrategy _strategy = new NoRetryStrategy();
        public IRetryStrategy Strategy
        {
            get => _strategy;
            set => _strategy = value ?? new NoRetryStrategy();
        }

        public int InitialDelaySeconds { get; set; } = 5;
        public int? MaxDelaySeconds { get; set; }
        public bool RetryOnTimeout { get; set; } = false;
        public bool RetryOnAnyError { get; set; } = false;

        public IReadOnlyList<string> RetryableErrorMessages => _retryableErrorMessages;
        public IReadOnlyList<int> RetryableExitCodes => _retryableExitCodes;

        /// <summary>
        /// ctor с Fluent API для удобства
        /// </summary>
        /// <param name="attempts"></param>
        /// <returns></returns>
        public RetrySettings WithMaxAttempts(int attempts)
        {
            MaxAttempts = attempts;
            return this;
        }

        public RetrySettings WithStrategy(RetryStrategy strategyType)
        {
            Strategy = RetryStrategyFactory.Create(strategyType);
            return this;
        }

        public RetrySettings WithStrategy(IRetryStrategy strategy)
        {
            Strategy = strategy;
            return this;
        }

        public RetrySettings WithInitialDelay(int seconds)
        {
            InitialDelaySeconds = seconds;
            return this;
        }

        public RetrySettings RetryOnErrors(params string[] errors)
        {
            _retryableErrorMessages.AddRange(errors);
            return this;
        }

        public RetrySettings RetryOnExitCodes(params int[] codes)
        {
            _retryableExitCodes.AddRange(codes);
            return this;
        }
    }
}
