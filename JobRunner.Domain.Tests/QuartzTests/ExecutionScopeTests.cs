using FluentAssertions;
using JobRunner.Core.DefaultImplementations;
using JobRunner.Core.Events.TaskEvents.TaskStatus;
using JobRunner.Core.Interfaces.Core;
using JobRunner.Core.Interfaces.Entities.JobTaskSettings.ScheduleSettings;
using JobRunner.Core.Interfaces.Events.DomainEvent;
using JobRunner.Core.Interfaces.Services.EntityServices;
using JobRunner.Core.Results;
using JobRunner.Quartz;
using Microsoft.Extensions.Logging;
using Moq;

namespace JobRunner.Domain.Tests.QuartzTests
{
    public class ExecutionScopeTests
    {
        private readonly Mock<ILogger> _loggerMock;
        private readonly Mock<IEncryptionService> _encryptionMock;
        private readonly Mock<IDomainEventDispatcher> _dispatcherMock;
        private readonly Mock<IJobTaskService<JobTaskExample, Guid>> _storageMock;
        private readonly JobTaskExample _task;

        public ExecutionScopeTests()
        {
            _loggerMock = new Mock<ILogger>();
            _encryptionMock = new Mock<IEncryptionService>();
            _dispatcherMock = new Mock<IDomainEventDispatcher>();
            _storageMock = new Mock<IJobTaskService<JobTaskExample, Guid>>();
            _task = new JobTaskExample
            {
                Id = Guid.NewGuid(),
                Name = "Test Task",
                ScheduleArguments = new ScheduleArgumentsExample(),
                JobTaskMetadata = new JobTaskMetadataExample(),
               // NotifySettings = new NotifySettingsExample()
            };
        }

        private ExecutionScope<JobTaskExample, Guid> CreateScope()
        {
            return new ExecutionScope<JobTaskExample, Guid>(_task, _loggerMock.Object, _dispatcherMock.Object);
        }

        [Fact]
        public async Task DecryptArgumentsAsync_ShouldCallDecryptionAndSetFlag()
        {
            // Arrange
            var scope = CreateScope();

            // Act
            await scope.DecryptArgumentsAsync(_encryptionMock.Object);

            // Assert
            _encryptionMock.Verify(x =>
                x.DecryptSensitiveArgumentsAsync(_task.ScheduleArguments, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task ReencryptArgumentsAsync_WhenNotDecrypted_ShouldNotEncrypt()
        {
            // Arrange
            var scope = CreateScope();

            // Act
            await scope.ReencryptArgumentsAsync(_encryptionMock.Object);

            // Assert
            _encryptionMock.Verify(x =>
                x.ReencryptSensitiveArgumentsAsync(It.IsAny<IScheduleArguments>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task ReencryptArgumentsAsync_WhenDecrypted_ShouldEncryptAndResetFlag()
        {
            // Arrange
            var scope = CreateScope();

            // Act
            await scope.DecryptArgumentsAsync(_encryptionMock.Object);
            await scope.ReencryptArgumentsAsync(_encryptionMock.Object);
            await scope.ReencryptArgumentsAsync(_encryptionMock.Object);

            // Assert
            _encryptionMock.Verify(x =>
                x.ReencryptSensitiveArgumentsAsync(It.IsAny<IScheduleArguments>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task PublishStartedEventAsync_ShouldSetStartTimeAndPublish()
        {
            // Arrange
            var scope = CreateScope();
            var beforeTime = DateTime.UtcNow;

            // Act
            await scope.PublishStartedEventAsync(_dispatcherMock.Object);

            // Assert
            scope.StartTime.Should().BeAfter(beforeTime);
            _dispatcherMock.Verify(x =>
                x.PublishAsync(It.IsAny<TaskStartedEvent<Guid>>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task UpdateBeforeExecutionAsync_ShouldSetRunningFlagAndUpdate()
        {
            // Arrange
            var scope = CreateScope();

            // Act
            await scope.PublishStartedEventAsync(_dispatcherMock.Object);
            await scope.UpdateBeforeExecutionAsync(_storageMock.Object);

            // Assert
            _task.JobTaskMetadata.IsRunning.Should().BeTrue();
            _task.JobTaskMetadata.LastRun.Should().Be(scope.StartTime);
            _task.StartRun.Should().Be(scope.StartTime);
            _storageMock.Verify(x => x.UpdateAsync(_task, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAfterExecutionAsync_WhenSuccessful_ShouldIncrementCounters()
        {
            // Arrange
            var scope = CreateScope();
            _task.JobTaskMetadata = new JobTaskMetadataExample();
            var result = JobExecutionResult.CreateSuccess(12345, DateTime.UtcNow);

            // Act
            await scope.PublishStartedEventAsync(_dispatcherMock.Object);
            await scope.UpdateAfterExecutionAsync(result, _storageMock.Object);

            // Assert
            _task.JobTaskMetadata.TotalRunCount.Should().Be(1);
            _task.JobTaskMetadata.SuccessCount.Should().Be(1);
            _task.JobTaskMetadata.ConsecutiveFailures.Should().Be(0);
            _task.JobTaskMetadata.IsRunning.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateAfterExecutionAsync_WhenFailed_ShouldIncrementFailureCount()
        {
            // Arrange
            var scope = CreateScope();
            _task.JobTaskMetadata = new JobTaskMetadataExample();
            var result = JobExecutionResult.CreateFailure("Test error", DateTime.UtcNow);

            // Act
            await scope.PublishStartedEventAsync(_dispatcherMock.Object);
            await scope.UpdateAfterExecutionAsync(result, _storageMock.Object);

            // Assert
            _task.JobTaskMetadata.TotalRunCount.Should().Be(1);
            _task.JobTaskMetadata.FailureCount.Should().Be(1);
            _task.JobTaskMetadata.ConsecutiveFailures.Should().Be(1);
            _task.JobTaskMetadata.LastError.Should().Be("Test error");
            _task.JobTaskMetadata.IsRunning.Should().BeFalse();
        }

        [Fact]
        public async Task HandleCancellationAsync_ShouldUpdateMetadataAndPublish()
        {
            // Arrange
            var scope = CreateScope();
            _task.JobTaskMetadata = new JobTaskMetadataExample { IsRunning = true };
            var exception = new OperationCanceledException();

            // Act
            await scope.HandleCancellationAsync(exception, _storageMock.Object, _dispatcherMock.Object);

            // Assert
            _task.JobTaskMetadata.IsRunning.Should().BeFalse();
            _task.JobTaskMetadata.FailureCount.Should().Be(1);
            _task.JobTaskMetadata.ConsecutiveFailures.Should().Be(1);
            _task.JobTaskMetadata.LastError.Should().Be("Execution was cancelled");
            _storageMock.Verify(x => x.UpdateAsync(_task, It.IsAny<CancellationToken>()), Times.Once);
            _dispatcherMock.Verify(x => x.PublishAsync(It.IsAny<TaskCompletedEvent<Guid>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleFailureAsync_ShouldUpdateMetadataAndPublish()
        {
            // Arrange
            var scope = CreateScope();
            _task.JobTaskMetadata = new JobTaskMetadataExample { IsRunning = true };
            var exception = new InvalidOperationException("Something went wrong");

            // Act
            await scope.HandleFailureAsync(exception, _storageMock.Object, _dispatcherMock.Object);

            // Assert
            _task.JobTaskMetadata.IsRunning.Should().BeFalse();
            _task.JobTaskMetadata.FailureCount.Should().Be(1);
            _task.JobTaskMetadata.ConsecutiveFailures.Should().Be(1);
            _task.JobTaskMetadata.LastError.Should().Be("Something went wrong");
            _storageMock.Verify(x => x.UpdateAsync(_task, It.IsAny<CancellationToken>()), Times.Once);
            _dispatcherMock.Verify(x => x.PublishAsync(It.IsAny<TaskCompletedEvent<Guid>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ReencryptArgumentsAsync_WhenEncryptionFails_ShouldLogCritical()
        {
            // Arrange
            var scope = CreateScope();
            _encryptionMock
                .Setup(x => x.ReencryptSensitiveArgumentsAsync(It.IsAny<IScheduleArguments>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Encryption failed"));

            // Act
            await scope.DecryptArgumentsAsync(_encryptionMock.Object);

            // Act & Assert
            Func<Task> act = async () => await scope.ReencryptArgumentsAsync(_encryptionMock.Object);
            await act.Should().NotThrowAsync();

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Critical,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Failed to re-encrypt")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}