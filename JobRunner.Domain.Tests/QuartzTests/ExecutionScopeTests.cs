using FluentAssertions;
using JobRunner.Core.Entities;
using JobRunner.Core.Entities.ValueObjects;
using JobRunner.Core.Events;
using JobRunner.Core.Events.TaskEvents.TaskStatus;
using JobRunner.Core.Interfaces.Core;
using JobRunner.Core.Interfaces.EntityServices;
using JobRunner.Core.Results;
using Microsoft.Extensions.Logging;
using Moq;
using JobRunner.Quartz;

namespace JobRunner.Domain.Tests.QuartzTests
{
    public class ExecutionScopeTests
    {
        private readonly Mock<ILogger> _loggerMock;
        private readonly Mock<IEncryptionService> _encryptionMock;
        private readonly Mock<IDomainEventDispatcher> _dispatcherMock;
        private readonly Mock<ITaskService<IJobTask>> _storageMock;
        private readonly Mock<IJobTask> _taskMock;

        public ExecutionScopeTests()
        {
            _loggerMock = new Mock<ILogger>();
            _encryptionMock = new Mock<IEncryptionService>();
            _dispatcherMock = new Mock<IDomainEventDispatcher>();
            _storageMock = new Mock<ITaskService<IJobTask>>();
            _taskMock = new Mock<IJobTask>();

            _taskMock.Setup(x => x.Id).Returns(Guid.NewGuid());
            _taskMock.Setup(x => x.Name).Returns("Test Task");
            _taskMock.Setup(x => x.ScheduleArguments).Returns(new Mock<IScheduleArguments>().Object);
            _taskMock.Setup(x => x.JobTaskMetadata).Returns(new Mock<IJobTaskMetadata>().Object);
        }

        [Fact]
        public async Task DecryptArgumentsAsync_ShouldCallDecryptionAndSetFlag()
        {
            var scope = new ExecutionScope(_taskMock.Object, _loggerMock.Object);

            await scope.DecryptArgumentsAsync(_encryptionMock.Object);

            _encryptionMock.Verify(x =>
                x.DecryptSensitiveArgumentsAsync(_taskMock.Object.ScheduleArguments, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task ReencryptArgumentsAsync_WhenNotDecrypted_ShouldNotEncrypt()
        {
            var scope = new ExecutionScope(_taskMock.Object, _loggerMock.Object);

            await scope.ReencryptArgumentsAsync(_encryptionMock.Object);

            _encryptionMock.Verify(x =>
                x.ReencryptSensitiveArgumentsAsync(It.IsAny<IScheduleArguments>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task ReencryptArgumentsAsync_WhenDecrypted_ShouldEncryptAndResetFlag()
        {
            var scope = new ExecutionScope(_taskMock.Object, _loggerMock.Object);

            await scope.DecryptArgumentsAsync(_encryptionMock.Object);
            await scope.ReencryptArgumentsAsync(_encryptionMock.Object);
            await scope.ReencryptArgumentsAsync(_encryptionMock.Object); 

            _encryptionMock.Verify(x =>
                x.ReencryptSensitiveArgumentsAsync(It.IsAny<IScheduleArguments>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task PublishStartedEventAsync_ShouldSetStartTimeAndPublish()
        {
            var scope = new ExecutionScope(_taskMock.Object, _loggerMock.Object);
            var beforeTime = DateTime.UtcNow;

            await scope.PublishStartedEventAsync(_dispatcherMock.Object);

            scope.StartTime.Should().BeAfter(beforeTime);
            _dispatcherMock.Verify(x =>
                x.PublishAsync(It.IsAny<TaskStartedEvent>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task UpdateBeforeExecutionAsync_ShouldSetRunningFlagAndUpdate()
        {
            var metadataMock = new Mock<IJobTaskMetadata>();
            _taskMock.Setup(x => x.JobTaskMetadata).Returns(metadataMock.Object);

            var scope = new ExecutionScope(_taskMock.Object, _loggerMock.Object);
            await scope.PublishStartedEventAsync(_dispatcherMock.Object);

            await scope.UpdateBeforeExecutionAsync(_storageMock.Object);

            metadataMock.VerifySet(x => x.IsRunning = true, Times.Once);
            metadataMock.VerifySet(x => x.LastRun = scope.StartTime, Times.Once);
            _taskMock.VerifySet(x => x.StartRun = scope.StartTime, Times.Once);
            _storageMock.Verify(x => x.UpdateAsync(_taskMock.Object, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAfterExecutionAsync_WhenSuccessful_ShouldIncrementCounters()
        {
            var metadataMock = new Mock<IJobTaskMetadata>();
            metadataMock.SetupProperty(x => x.TotalRunCount, 0);
            metadataMock.SetupProperty(x => x.SuccessCount, 0);
            metadataMock.SetupProperty(x => x.ConsecutiveFailures, 3);
            metadataMock.SetupProperty(x => x.IsRunning, true);
            _taskMock.Setup(x => x.JobTaskMetadata).Returns(metadataMock.Object);

            var result = JobExecutionResult.CreateSuccess(12345, DateTime.UtcNow);
            var scope = new ExecutionScope(_taskMock.Object, _loggerMock.Object);

            await scope.PublishStartedEventAsync(_dispatcherMock.Object);
            await scope.UpdateAfterExecutionAsync(result, _storageMock.Object);

            metadataMock.Object.TotalRunCount.Should().Be(1);
            metadataMock.Object.SuccessCount.Should().Be(1);
            metadataMock.Object.ConsecutiveFailures.Should().Be(0);
            metadataMock.Object.IsRunning.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateAfterExecutionAsync_WhenFailed_ShouldIncrementFailureCount()
        {
            var metadataMock = new Mock<IJobTaskMetadata>();
            metadataMock.SetupProperty(x => x.TotalRunCount, 0);
            metadataMock.SetupProperty(x => x.FailureCount, 0);
            metadataMock.SetupProperty(x => x.ConsecutiveFailures, 0);
            metadataMock.SetupProperty(x => x.IsRunning, true);
            _taskMock.Setup(x => x.JobTaskMetadata).Returns(metadataMock.Object);

            var result = JobExecutionResult.CreateFailure("Test error", DateTime.UtcNow);
            var scope = new ExecutionScope(_taskMock.Object, _loggerMock.Object);

            await scope.PublishStartedEventAsync(_dispatcherMock.Object);
            await scope.UpdateAfterExecutionAsync(result, _storageMock.Object);

            metadataMock.Object.TotalRunCount.Should().Be(1);
            metadataMock.Object.FailureCount.Should().Be(1);
            metadataMock.Object.ConsecutiveFailures.Should().Be(1);
            metadataMock.Object.LastError.Should().Be("Test error");
            metadataMock.Object.IsRunning.Should().BeFalse();
        }

        [Fact]
        public async Task HandleCancellationAsync_ShouldUpdateMetadataAndPublish()
        {
            var metadataMock = new Mock<IJobTaskMetadata>();
            metadataMock.SetupProperty(x => x.IsRunning, true);
            metadataMock.SetupProperty(x => x.FailureCount, 0);
            metadataMock.SetupProperty(x => x.ConsecutiveFailures, 0);
            _taskMock.Setup(x => x.JobTaskMetadata).Returns(metadataMock.Object);

            var scope = new ExecutionScope(_taskMock.Object, _loggerMock.Object);
            var exception = new OperationCanceledException();

            await scope.HandleCancellationAsync(exception, _storageMock.Object, _dispatcherMock.Object);

            metadataMock.Object.IsRunning.Should().BeFalse();
            metadataMock.Object.FailureCount.Should().Be(1);
            metadataMock.Object.ConsecutiveFailures.Should().Be(1);
            metadataMock.Object.LastError.Should().Be("Execution was cancelled");
            _storageMock.Verify(x => x.UpdateAsync(_taskMock.Object, It.IsAny<CancellationToken>()), Times.Once);
            _dispatcherMock.Verify(x => x.PublishAsync(It.IsAny<TaskCompletedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleFailureAsync_ShouldUpdateMetadataAndPublish()
        {
            var metadataMock = new Mock<IJobTaskMetadata>();
            metadataMock.SetupProperty(x => x.IsRunning, true);
            metadataMock.SetupProperty(x => x.FailureCount, 0);
            metadataMock.SetupProperty(x => x.ConsecutiveFailures, 0);
            _taskMock.Setup(x => x.JobTaskMetadata).Returns(metadataMock.Object);

            var scope = new ExecutionScope(_taskMock.Object, _loggerMock.Object);
            var exception = new InvalidOperationException("Something went wrong");

            await scope.HandleFailureAsync(exception, _storageMock.Object, _dispatcherMock.Object);

            metadataMock.Object.IsRunning.Should().BeFalse();
            metadataMock.Object.FailureCount.Should().Be(1);
            metadataMock.Object.ConsecutiveFailures.Should().Be(1);
            metadataMock.Object.LastError.Should().Be("Something went wrong");
            _storageMock.Verify(x => x.UpdateAsync(_taskMock.Object, It.IsAny<CancellationToken>()), Times.Once);
            _dispatcherMock.Verify(x => x.PublishAsync(It.IsAny<TaskCompletedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ReencryptArgumentsAsync_WhenEncryptionFails_ShouldLogCritical()
        {
            var scope = new ExecutionScope(_taskMock.Object, _loggerMock.Object);
            _encryptionMock
                .Setup(x => x.ReencryptSensitiveArgumentsAsync(It.IsAny<IScheduleArguments>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Encryption failed"));

            await scope.DecryptArgumentsAsync(_encryptionMock.Object);
            await scope.ReencryptArgumentsAsync(_encryptionMock.Object);
        }
    }
}
