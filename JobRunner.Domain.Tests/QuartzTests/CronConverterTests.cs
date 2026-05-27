using JobRunner.Core.Entities.ValueObjects.Settings;
using JobRunner.Quartz.Converters;
using FluentAssertions;
using Xunit;
using Moq;
using JobRunner.Core.Entities.ValueObjects;

namespace JobRunner.Domain.Tests.QuartzTests
{
    public class CronConverterTests
    {
        private readonly CronConverter _converter = new();

        [Fact]
        public void Convert_DailySchedule_ReturnsCorrectCron()
        {
            var schedule = new DailySchedule { Hour = 14, Minute = 30 };

            var result = _converter.Convert(schedule);

            result.Should().Be("0 30 14 * * ?");
        }

        [Fact]
        public void Convert_WeeklySchedule_ReturnsCorrectCron()
        {
            var schedule = new WeeklySchedule
            {
                Hour = 9,
                Minute = 0,
                DaysOfWeek = { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday }
            };

            var result = _converter.Convert(schedule);

            // В Quartz: 1=Sunday, 2=Monday, 3=Tuesday, 4=Wednesday, 5=Thursday, 6=Friday, 7=Saturday
            result.Should().Be("0 0 9 ? * 2,4,6");
        }

        [Fact]
        public void Convert_IntervalSchedule_30Minutes_ReturnsCorrectCron()
        {
            var schedule = new IntervalSchedule { IntervalMinutes = 30 };

            var result = _converter.Convert(schedule);

            result.Should().Be("0 */30 * * * ?");
        }

        [Fact]
        public void Convert_IntervalSchedule_60Minutes_ReturnsHourlyCron()
        {
            var schedule = new IntervalSchedule { IntervalMinutes = 60 };

            var result = _converter.Convert(schedule);

            // 0 */60 * * * ? и 0 0 * * * ? эквивалентны, но второй читаемее
            result.Should().Be("0 */60 * * * ?");
        }

        [Fact]
        public void Convert_OnceSchedule_ReturnsSpecificDateCron()
        {
            var startTime = new DateTime(2026, 12, 25, 10, 30, 15);
            var schedule = new OnceSchedule { StartTime = startTime };

            var result = _converter.Convert(schedule);

            result.Should().Be("15 30 10 25 12 ? 2026");
        }

        [Fact]
        public void Convert_UnsupportedSchedule_ThrowsNotSupportedException()
        {
            var schedule = new Mock<IScheduleSettings>();

            var act = () => _converter.Convert(schedule.Object);

            act.Should().Throw<NotSupportedException>();
        }
    }
}
