using JobRunner.Core.DefaultImplementations.Strategies;
using JobRunner.Core.Entities.Enums;
using JobRunner.Core.Interfaces.Entities.JobTaskSettings;
using JobRunner.Core.Interfaces.Entities.Retry;
using System.Collections.Generic;

namespace JobRunner.Core.DefaultImplementations
{
    /// <summary>
    /// Настройки политики повторных попыток с Fluent API
    /// </summary>
    public class RetrySettingsExample : IRetrySettings
    {
        private readonly List<string> _retryableErrorMessages = new();
        private readonly List<int> _retryableExitCodes = new();

        public int MaxAttempts { get; set; } = 1;

        private IRetryPolicy _retryPolicy = new NoRetryStrategy();
        public IRetryPolicy RetryPolicy
        {
            get => _retryPolicy;
            set => _retryPolicy = value ?? new NoRetryStrategy();
        }

        public int InitialDelaySeconds { get; set; } = 5;
        public int? MaxDelaySeconds { get; set; }

        private bool _retryOnTimeout = false;
        public bool RetryOnTimeout
        {
            get => _retryOnTimeout;
            set => _retryOnTimeout = value;
        }

        private bool _retryOnAnyError = false;
        public bool RetryOnAnyError
        {
            get => _retryOnAnyError;
            set => _retryOnAnyError = value;
        }

        public IReadOnlyList<string> RetryableErrorMessages => _retryableErrorMessages;
        public IReadOnlyList<int> RetryableExitCodes => _retryableExitCodes;

        #region Fluent API Methods

        /// <summary>
        /// Устанавливает максимальное количество попыток
        /// </summary>
        public RetrySettingsExample WithMaxAttempts(int attempts)
        {
            MaxAttempts = attempts;
            return this;
        }

        /// <summary>
        /// Устанавливает стратегию повторов по типу
        /// </summary>
        public RetrySettingsExample WithStrategy(RetryStrategyTypes strategyType)
        {
            RetryPolicy = RetryStrategyFactory.Create(strategyType);
            return this;
        }

        /// <summary>
        /// Устанавливает кастомную стратегию повторов
        /// </summary>
        public RetrySettingsExample WithStrategy(IRetryPolicy retryPolicy)
        {
            RetryPolicy = retryPolicy;
            return this;
        }

        /// <summary>
        /// Устанавливает начальную задержку (в секундах)
        /// </summary>
        public RetrySettingsExample WithInitialDelay(int seconds)
        {
            InitialDelaySeconds = seconds;
            return this;
        }

        /// <summary>
        /// Устанавливает максимальную задержку (в секундах)
        /// </summary>
        public RetrySettingsExample WithMaxDelaySeconds(int seconds)
        {
            MaxDelaySeconds = seconds;
            return this;
        }

        /// <summary>
        /// Устанавливает флаг повтора при таймауте (Fluent API)
        /// </summary>
        public RetrySettingsExample SetRetryOnTimeout(bool retryOnTimeout)
        {
            RetryOnTimeout = retryOnTimeout;
            return this;
        }

        /// <summary>
        /// Устанавливает флаг повтора при любой ошибке (Fluent API)
        /// </summary>
        public RetrySettingsExample SetRetryOnAnyError(bool retryOnAnyError)
        {
            RetryOnAnyError = retryOnAnyError;
            return this;
        }

        /// <summary>
        /// Добавляет сообщения об ошибках для повтора
        /// </summary>
        public RetrySettingsExample RetryOnErrors(params string[] errors)
        {
            _retryableErrorMessages.AddRange(errors);
            return this;
        }

        /// <summary>
        /// Добавляет коды выхода для повтора
        /// </summary>
        public RetrySettingsExample RetryOnExitCodes(params int[] codes)
        {
            _retryableExitCodes.AddRange(codes);
            return this;
        }

        #endregion
    }
}