using FluentAssertions;
using JobRunner.Core.DefaultImplementations;
using JobRunner.Core.Entities.ValueObjects.Settings;
using JobRunner.Core.Interfaces.Converters;
using Moq;
using Quartz;
using JobRunner.Quartz.Scheduler;

namespace JobRunner.Domain.Tests.QuartzTests
{
    public class QuartzSchedulerTests
    {
        private readonly Mock<IScheduler> _schedulerMock;
        private readonly Mock<IScheduleConverter> _converterMock;
        private readonly QuartzScheduler<JobTask, Guid> _scheduler;

        public QuartzSchedulerTests()
        {
            _schedulerMock = new Mock<IScheduler>();
            _converterMock = new Mock<IScheduleConverter>();
            _scheduler = new QuartzScheduler<JobTask, Guid>(_schedulerMock.Object, _converterMock.Object);
        }

        [Fact]
        public async Task RunNowAsync_ShouldTriggerJob()
        {
            var taskId = Guid.NewGuid();

            await _scheduler.RunNowAsync(taskId);

            _schedulerMock.Verify(x =>
                x.TriggerJob(
                    It.Is<JobKey>(key => key.Name == taskId.ToString()),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task PauseAsync_ShouldPauseJob()
        {
            var taskId = Guid.NewGuid();

            await _scheduler.PauseAsync(taskId);

            _schedulerMock.Verify(x =>
                x.PauseJob(
                    It.Is<JobKey>(key => key.Name == taskId.ToString()),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task ResumeAsync_ShouldResumeJob()
        {
            var taskId = Guid.NewGuid();

            await _scheduler.ResumeAsync(taskId);

            _schedulerMock.Verify(x =>
                x.ResumeJob(
                    It.Is<JobKey>(key => key.Name == taskId.ToString()),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task StopAsync_ShouldInterruptJob()
        {
            var taskId = Guid.NewGuid();

            await _scheduler.StopAsync(taskId);

            _schedulerMock.Verify(x =>
                x.Interrupt(
                    It.Is<JobKey>(key => key.Name == taskId.ToString()),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task RestartAsync_ShouldStopThenRun()
        {
            var taskId = Guid.NewGuid();

            await _scheduler.RestartAsync(taskId, delay: 10);

            _schedulerMock.Verify(x => x.Interrupt(It.Is<JobKey>(key => key.Name == taskId.ToString()), It.IsAny<CancellationToken>()), Times.Once);
            _schedulerMock.Verify(x => x.TriggerJob(It.Is<JobKey>(key => key.Name == taskId.ToString()), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ScheduleAsync_ShouldScheduleJobWithTrigger()
        {
            var taskId = Guid.NewGuid();
            var schedule = new DailySchedule { Hour = 14, Minute = 30 };
            var cronExpression = "0 30 14 * * ?";

            _converterMock.Setup(x => x.Convert(schedule)).Returns(cronExpression);

            await _scheduler.ScheduleAsync(taskId, schedule);

            _schedulerMock.Verify(x =>
                x.ScheduleJob(
                    It.IsAny<IJobDetail>(),
                    It.IsAny<ITrigger>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task ScheduleAsync_WhenCronExpressionEmpty_ShouldThrow()
        {
            var taskId = Guid.NewGuid();
            var schedule = new DailySchedule();

            _converterMock.Setup(x => x.Convert(schedule)).Returns(string.Empty);

            var act = async () => await _scheduler.ScheduleAsync(taskId, schedule);

            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task UnscheduleAsync_ShouldDeleteJob()
        {
            var taskId = Guid.NewGuid();

            await _scheduler.UnscheduleAsync(taskId);

            _schedulerMock.Verify(x =>
                x.DeleteJob(
                    It.Is<JobKey>(key => key.Name == taskId.ToString()),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task RestoreSchedulesAsync_ShouldScheduleOnlyEnabledTasks()
        {
            var enabledTask = new JobTask
            {
                Id = Guid.NewGuid(),
                IsEnabled = true,
                ScheduleSettings = new DailySchedule()
            };

            var disabledTask = new JobTask
            {
                Id = Guid.NewGuid(),
                IsEnabled = false,
                ScheduleSettings = new DailySchedule()
            };

            var tasks = new[] { enabledTask, disabledTask };

            _converterMock.Setup(x => x.Convert(It.IsAny<DailySchedule>())).Returns("0 0 9 * * ?");

            await _scheduler.RestoreSchedulesAsync(tasks);

            _schedulerMock.Verify(x =>
                x.ScheduleJob(
                    It.IsAny<IJobDetail>(),
                    It.IsAny<ITrigger>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}