using JobRunner.Quartz.Converters;
using FluentAssertions;
using Moq;
using JobRunner.Core.Entities.ValueObjects;
using JobRunner.Core.Interfaces.Entities.JobTaskSettings.ScheduleSettings;

namespace JobRunner.Domain.Tests.QuartzTests
{
    public class CronConverterTests
    {
        private readonly CronConverter _converter = new();

        [Fact]
        public void Convert_OnceSchedule_ReturnsCorrectCron()
        {
            var startTime = new DateTime(2026, 12, 25, 10, 30, 15);
            var schedule = new OnceSchedule { StartTime = startTime };

            var result = _converter.Convert(schedule);

            result.Should().Be("30 10 25 12 ? 2026");
        }

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

            result.Should().Be("0 0 9 ? * 2,4,6");
        }

        [Fact]
        public void Convert_WeeklySchedule_SingleDay_ReturnsCorrectCron()
        {
            var schedule = new WeeklySchedule
            {
                Hour = 8,
                Minute = 30,
                DaysOfWeek = { DayOfWeek.Sunday }
            };

            var result = _converter.Convert(schedule);

            result.Should().Be("0 30 8 ? * 1");
        }

        [Fact]
        public void Convert_IntervalSchedule_Every5Minutes_ReturnsCorrectCron()
        {
            var schedule = new IntervalSchedule { IntervalMinutes = 5 };

            var result = _converter.Convert(schedule);

            result.Should().Be("0 */5 * * * ?");
        }

        [Fact]
        public void Convert_IntervalSchedule_Every30Minutes_ReturnsCorrectCron()
        {
            var schedule = new IntervalSchedule { IntervalMinutes = 30 };

            var result = _converter.Convert(schedule);

            result.Should().Be("0 */30 * * * ?");
        }

        [Fact]
        public void Convert_IntervalSchedule_Every60Minutes_ReturnsCorrectCron()
        {
            var schedule = new IntervalSchedule { IntervalMinutes = 60 };

            var result = _converter.Convert(schedule);

            result.Should().Be("0 */60 * * * ?");
        }

        [Fact]
        public void Convert_MonthlySchedule_ReturnsCorrectCron()
        {
            var schedule = new MonthlySchedule { Day = 15, Hour = 12, Minute = 0 };

            var result = _converter.Convert(schedule);

            result.Should().Be("0 0 12 15 * ?");
        }

        [Fact]
        public void Convert_QuarterlySchedule_Q1_ReturnsCorrectCron()
        {
            var schedule = new QuarterlySchedule { StartMonth = 1, Day = 1, Hour = 9, Minute = 0 };

            var result = _converter.Convert(schedule);

            result.Should().Be("0 0 9 1 1,2,3 ?");
        }

        [Fact]
        public void Convert_QuarterlySchedule_Q2_ReturnsCorrectCron()
        {
            var schedule = new QuarterlySchedule { StartMonth = 4, Day = 1, Hour = 10, Minute = 30 };

            var result = _converter.Convert(schedule);

            result.Should().Be("0 30 10 1 4,5,6 ?");
        }

        [Fact]
        public void Convert_QuarterlySchedule_Q3_ReturnsCorrectCron()
        {
            var schedule = new QuarterlySchedule { StartMonth = 7, Day = 15, Hour = 14, Minute = 0 };

            var result = _converter.Convert(schedule);

            result.Should().Be("0 0 14 15 7,8,9 ?");
        }

        [Fact]
        public void Convert_QuarterlySchedule_Q4_ReturnsCorrectCron()
        {
            var schedule = new QuarterlySchedule { StartMonth = 10, Day = 31, Hour = 23, Minute = 59 };

            var result = _converter.Convert(schedule);

            result.Should().Be("0 59 23 31 10,11,12 ?");
        }

        [Fact]
        public void Convert_YearlySchedule_ReturnsCorrectCron()
        {
            var schedule = new YearlySchedule { Month = 6, Day = 1, Hour = 9, Minute = 0 };

            var result = _converter.Convert(schedule);

            result.Should().Be("0 0 9 1 6 ?");
        }

        [Fact]
        public void Convert_EveryMinutesSchedule_ReturnsCorrectCron()
        {
            var schedule = new EveryMinutesSchedule { IntervalMinutes = 15 };

            var result = _converter.Convert(schedule);

            result.Should().Be("0 */15 * * * ?");
        }

        [Fact]
        public void Convert_HourlySchedule_EveryHour_ReturnsCorrectCron()
        {
            var schedule = new HourlySchedule { HourInterval = 1, Minute = 0 };

            var result = _converter.Convert(schedule);

            result.Should().Be("0 0 * * * ?");
        }

        [Fact]
        public void Convert_HourlySchedule_Every2Hours_ReturnsCorrectCron()
        {
            var schedule = new HourlySchedule { HourInterval = 2, Minute = 30 };

            var result = _converter.Convert(schedule);

            result.Should().Be("0 30 */2 * * ?");
        }

        [Fact]
        public void Convert_NullSchedule_ThrowsArgumentNullException()
        {
            var act = () => _converter.Convert(null!);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Convert_UnsupportedSchedule_ThrowsNotSupportedException()
        {
            var schedule = new Mock<IScheduleSettings>();

            var act = () => _converter.Convert(schedule.Object);

            act.Should().Throw<NotSupportedException>()
                .WithMessage($"Unsupported schedule type: {schedule.Object.GetType()}");
        }
    }
}
