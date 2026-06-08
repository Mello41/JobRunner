using FluentAssertions;
using JobRunner.Core.DefaultImplementations;
using JobRunner.Core.DefaultImplementations.Strategies;

namespace JobRunner.Domain.Tests.CoreTests.RetryTests
{
    public class RetryStrategyTests
    {
        private readonly RetrySettings _settings;

        public RetryStrategyTests()
        {
            _settings = new RetrySettings()
                .WithMaxAttempts(3)
                .WithInitialDelay(5);
        }

        [Fact]
        public void FixedDelayStrategy_ReturnsSameDelayForEachAttempt()
        {
            var strategy = new FixedDelayStrategy();

            var delay1 = strategy.GetDelayMilliseconds(2, _settings);
            var delay2 = strategy.GetDelayMilliseconds(3, _settings);

            delay1.Should().Be(5000);
            delay2.Should().Be(5000);
        }

        [Fact]
        public void ExponentialBackoffStrategy_DelayDoublesEachAttempt()
        {
            var strategy = new ExponentialBackoffStrategy();

            var delay1 = strategy.GetDelayMilliseconds(2, _settings); // 5 * 2^1 = 10
            var delay2 = strategy.GetDelayMilliseconds(3, _settings); // 5 * 2^2 = 20
            var delay3 = strategy.GetDelayMilliseconds(4, _settings); // 5 * 2^3 = 40

            delay1.Should().Be(10000);
            delay2.Should().Be(20000);
            delay3.Should().Be(40000);
        }

        [Fact]
        public void ExponentialBackoffStrategy_RespectsMaxDelay()
        {
            var settings = new RetrySettings()
                .WithMaxAttempts(5)
                .WithInitialDelay(10)
                .WithMaxDelaySeconds(30);

            var strategy = new ExponentialBackoffStrategy();

            var delay1 = strategy.GetDelayMilliseconds(4, settings); // 10 * 8 = 80 -> capped to 30
            var delay2 = strategy.GetDelayMilliseconds(5, settings); // 10 * 16 = 160 -> capped to 30

            delay1.Should().Be(30000);
            delay2.Should().Be(30000);
        }

        [Fact]
        public void IncrementalStrategy_DelayIncreasesLinearly()
        {
            var strategy = new IncrementalStrategy();

            var delay1 = strategy.GetDelayMilliseconds(2, _settings); // 5 * 2 = 10
            var delay2 = strategy.GetDelayMilliseconds(3, _settings); // 5 * 3 = 15
            var delay3 = strategy.GetDelayMilliseconds(4, _settings); // 5 * 4 = 20

            delay1.Should().Be(10000);
            delay2.Should().Be(15000);
            delay3.Should().Be(20000);
        }

        [Fact]
        public void ShouldRetry_RespectsMaxAttempts()
        {
            var strategy = new FixedDelayStrategy();

            var shouldRetry = strategy.ShouldRetry(
                "Any error", null, false,
                currentAttempt: 3, _settings);

            shouldRetry.Should().BeFalse();
        }

        [Fact]
        public void ShouldRetry_OnTimeout_WhenRetryOnTimeoutEnabled()
        {
            var settings = new RetrySettings()
                .WithMaxAttempts(3)
                .SetRetryOnTimeout(true); 

            var strategy = new FixedDelayStrategy();

            var shouldRetry = strategy.ShouldRetry(
                null, null, isTimeout: true,
                currentAttempt: 1, settings);

            shouldRetry.Should().BeTrue();
        }

        [Fact]
        public void ShouldRetry_OnTimeout_WhenRetryOnTimeoutDisabled()
        {
            var settings = new RetrySettings()
                .WithMaxAttempts(3)
                .SetRetryOnTimeout(false); 

            var strategy = new FixedDelayStrategy();

            var shouldRetry = strategy.ShouldRetry(
                null, null, isTimeout: true,
                currentAttempt: 1, settings);

            shouldRetry.Should().BeFalse();
        }

        [Fact]
        public void ShouldRetry_OnSpecificError_WhenErrorMatches()
        {
            var settings = new RetrySettings()
                .WithMaxAttempts(3)
                .RetryOnErrors("Connection timeout", "Database error");

            var strategy = new FixedDelayStrategy();

            var shouldRetry = strategy.ShouldRetry(
                "Connection timeout occurred", null, false,
                currentAttempt: 1, settings);

            shouldRetry.Should().BeTrue();
        }

        [Fact]
        public void ShouldRetry_OnSpecificExitCode_WhenCodeMatches()
        {
            var settings = new RetrySettings()
                .WithMaxAttempts(3)
                .RetryOnExitCodes(1, 100, 255);

            var strategy = new FixedDelayStrategy();

            var shouldRetry = strategy.ShouldRetry(
                null, exitCode: 100, false,
                currentAttempt: 1, settings);

            shouldRetry.Should().BeTrue();
        }

        [Fact]
        public void ShouldRetry_OnAnyError_WhenRetryOnAnyErrorEnabled()
        {
            var settings = new RetrySettings()
                .WithMaxAttempts(3)
                .SetRetryOnAnyError(true);

            var strategy = new FixedDelayStrategy();

            var shouldRetry = strategy.ShouldRetry(
                "Any random error", null, false,
                currentAttempt: 1, settings);

            shouldRetry.Should().BeTrue();
        }

        [Fact]
        public void ShouldRetry_ReturnsFalse_WhenNoConditionsMatch()
        {
            var settings = new RetrySettings()
                .WithMaxAttempts(3);

            var strategy = new FixedDelayStrategy();

            var shouldRetry = strategy.ShouldRetry(
                "Unknown error", null, false,
                currentAttempt: 1, settings);

            shouldRetry.Should().BeFalse();
        }

        [Fact]
        public void CanAlsoUsePropertiesDirectly()
        {
            var settings = new RetrySettings();
            settings.RetryOnTimeout = true;
            settings.RetryOnAnyError = false;

            settings.RetryOnTimeout.Should().BeTrue();
            settings.RetryOnAnyError.Should().BeFalse();
        }
    }
}