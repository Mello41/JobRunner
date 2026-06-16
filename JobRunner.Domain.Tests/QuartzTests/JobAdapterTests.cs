using FluentAssertions;
using JobRunner.Core.DefaultImplementations;
using JobRunner.Core.Events.TaskEvents.TaskStatus;
using JobRunner.Core.Interfaces.Core;
using JobRunner.Core.Interfaces.Entities;
using JobRunner.Core.Interfaces.Events.DomainEvent;
using JobRunner.Core.Interfaces.Execution;
using JobRunner.Core.Interfaces.Services.EntityServices;
using JobRunner.Core.Results;
using JobRunner.Quartz.Adapters;
using Microsoft.Extensions.Logging;
using Moq;
using Quartz;

namespace JobRunner.Domain.Tests.QuartzTests
{
    public class JobAdapterTests
    {
        private readonly Mock<IJobTaskService<JobTaskExample, Guid>> _storageMock;
        private readonly Mock<IJobExecutor<Guid>> _executorMock;
        private readonly Mock<IDomainEventDispatcher> _dispatcherMock;
        private readonly Mock<IEncryptionService> _encryptionMock;
        private readonly Mock<ILogger<JobAdapter<JobTaskExample, Guid>>> _loggerMock;
        private readonly JobAdapter<JobTaskExample, Guid> _adapter;

        public JobAdapterTests()
        {
            _storageMock = new Mock<IJobTaskService<JobTaskExample, Guid>>();
            _executorMock = new Mock<IJobExecutor<Guid>>();
            _dispatcherMock = new Mock<IDomainEventDispatcher>();
            _encryptionMock = new Mock<IEncryptionService>();
            _loggerMock = new Mock<ILogger<JobAdapter<JobTaskExample, Guid>>>();

            _adapter = new JobAdapter<JobTaskExample, Guid>(
                _storageMock.Object,
                _executorMock.Object,
                _dispatcherMock.Object,
                _encryptionMock.Object,
                _loggerMock.Object);
        }

        private static IJobExecutionContext CreateContext(Guid taskId)
        {
            var contextMock = new Mock<IJobExecutionContext>();
            var dataMap = new JobDataMap();
            dataMap.Put("TaskId", taskId.ToString());
            contextMock.Setup(x => x.MergedJobDataMap).Returns(dataMap);
            contextMock.Setup(x => x.CancellationToken).Returns(CancellationToken.None);
            return contextMock.Object;
        }

        private static JobTaskExample CreateTestTask(Guid? id = null, bool isEnabled = true, bool allowConcurrent = true, bool isRunning = false)
        {
            return new JobTaskExample
            {
                Id = id ?? Guid.NewGuid(),
                Name = "Test Task",
                ExecutionPath = "test.exe",
                IsEnabled = isEnabled,
                AllowConcurrentExecution = allowConcurrent,
                JobTaskMetadata = new JobTaskMetadataExample { IsRunning = isRunning },
                ScheduleArguments = new ScheduleArgumentsExample()
            };
        }

        [Fact]
        public async Task Execute_WhenTaskNotFound_ShouldLogWarning()
        {
            var taskId = Guid.NewGuid();
            _storageMock.Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((JobTaskExample?)null);

            await _adapter.Execute(CreateContext(taskId));

            _loggerMock.Verify(x =>
                x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("not found")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);

            _executorMock.Verify(x => x.ExecuteAsync(It.IsAny<IJobTask<Guid>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Execute_WhenTaskDisabled_ShouldSkipExecution()
        {
            var task = CreateTestTask(isEnabled: false);
            _storageMock.Setup(x => x.GetByIdAsync(task.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(task);

            await _adapter.Execute(CreateContext(task.Id));

            _executorMock.Verify(x => x.ExecuteAsync(It.IsAny<IJobTask<Guid>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Execute_WhenConcurrentDisabledAndTaskRunning_ShouldSkip()
        {
            var task = CreateTestTask(allowConcurrent: false, isRunning: true);
            _storageMock.Setup(x => x.GetByIdAsync(task.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(task);

            await _adapter.Execute(CreateContext(task.Id));

            _executorMock.Verify(x => x.ExecuteAsync(It.IsAny<IJobTask<Guid>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Execute_WhenConcurrentEnabled_ShouldRunEvenIfRunning()
        {
            var task = CreateTestTask(allowConcurrent: true, isRunning: true);
            _storageMock.Setup(x => x.GetByIdAsync(task.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(task);

            _executorMock.Setup(x => x.ExecuteAsync(It.IsAny<IJobTask<Guid>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(JobExecutionResult.CreateSuccess(123, DateTime.UtcNow));

            await _adapter.Execute(CreateContext(task.Id));

            _executorMock.Verify(x => x.ExecuteAsync(It.IsAny<IJobTask<Guid>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Execute_WhenTwoConcurrentExecutions_ShouldRunOnlyOne()
        {
            var task = CreateTestTask(allowConcurrent: false, isRunning: false);
            _storageMock.Setup(x => x.GetByIdAsync(task.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(task);

            var executorCalled = 0;
            _executorMock.Setup(x => x.ExecuteAsync(It.IsAny<IJobTask<Guid>>(), It.IsAny<CancellationToken>()))
                .Returns(async () =>
                {
                    Interlocked.Increment(ref executorCalled);
                    await Task.Delay(100);
                    return JobExecutionResult.CreateSuccess(123, DateTime.UtcNow);
                });

            var context = CreateContext(task.Id);
            var task1 = _adapter.Execute(context);
            var task2 = _adapter.Execute(context);

            await Task.WhenAll(task1, task2);

            executorCalled.Should().Be(1);
        }

        [Fact]
        public async Task Execute_WhenSuccessful_ShouldUpdateMetadata()
        {
            var task = CreateTestTask();
            _storageMock.Setup(x => x.GetByIdAsync(task.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(task);

            var result = JobExecutionResult.CreateSuccess(12345, DateTime.UtcNow);
            _executorMock.Setup(x => x.ExecuteAsync(It.IsAny<IJobTask<Guid>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            await _adapter.Execute(CreateContext(task.Id));

            _storageMock.Verify(x => x.UpdateAsync(It.IsAny<JobTaskExample>(), It.IsAny<CancellationToken>()), Times.AtLeast(2));
            _dispatcherMock.Verify(x => x.PublishAsync(It.IsAny<TaskStartedEvent<Guid>>(), It.IsAny<CancellationToken>()), Times.Once);
            _dispatcherMock.Verify(x => x.PublishAsync(It.IsAny<TaskCompletedEvent<Guid>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Execute_WhenExecutorThrows_ShouldHandleError()
        {
            var task = CreateTestTask();
            _storageMock.Setup(x => x.GetByIdAsync(task.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(task);

            _executorMock.Setup(x => x.ExecuteAsync(It.IsAny<IJobTask<Guid>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Execution failed"));

            var act = async () => await _adapter.Execute(CreateContext(task.Id));

            await act.Should().ThrowAsync<InvalidOperationException>();

            _storageMock.Verify(x => x.UpdateAsync(It.IsAny<JobTaskExample>(), It.IsAny<CancellationToken>()), Times.AtLeast(2));
            _dispatcherMock.Verify(x => x.PublishAsync(It.IsAny<TaskCompletedEvent<Guid>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Execute_ShouldDecryptAndReencryptArguments()
        {
            var task = CreateTestTask();
            _storageMock.Setup(x => x.GetByIdAsync(task.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(task);

            _executorMock.Setup(x => x.ExecuteAsync(It.IsAny<IJobTask<Guid>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(JobExecutionResult.CreateSuccess(123, DateTime.UtcNow));

            await _adapter.Execute(CreateContext(task.Id));

            _encryptionMock.Verify(x => x.DecryptSensitiveArgumentsAsync(task.ScheduleArguments, It.IsAny<CancellationToken>()), Times.Once);
            _encryptionMock.Verify(x => x.ReencryptSensitiveArgumentsAsync(task.ScheduleArguments, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}