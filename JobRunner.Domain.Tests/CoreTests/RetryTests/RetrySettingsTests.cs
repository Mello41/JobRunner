using FluentAssertions;
using JobRunner.Core.DefaultImplementations;

namespace JobRunner.Domain.Tests.CoreTests.RetryTests
{
    public class RetrySettingsTests
    {
        [Fact]
        public void RetrySettings_WithMaxAttempts_SetsCorrectValue()
        {
            var settings = new RetrySettingsExample()
                .WithMaxAttempts(5)
                .WithInitialDelay(10);

            settings.MaxAttempts.Should().Be(5);
            settings.InitialDelaySeconds.Should().Be(10);
        }

        [Fact]
        public void RetrySettings_RetryOnSpecificErrors_AddsToCollection()
        {
            var settings = new RetrySettingsExample()
                .RetryOnErrors("Connection refused", "Timeout", "Network error");

            settings.RetryableErrorMessages.Should().HaveCount(3);
            settings.RetryableErrorMessages.Should().Contain("Connection refused");
        }

        [Fact]
        public void RetrySettings_RetryOnExitCodes_AddsToCollection()
        {
            var settings = new RetrySettingsExample()
                .RetryOnExitCodes(1, 2, 100);

            settings.RetryableExitCodes.Should().HaveCount(3);
            settings.RetryableExitCodes.Should().Contain(1);
        }
    }
}
