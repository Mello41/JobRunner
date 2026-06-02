using FluentAssertions;
using JobRunner.Core.DefaultImplementations;
using JobRunner.Core.Entities;
using JobRunner.Core.Entities.ValueObjects;
using JobRunner.Core.Events;
using JobRunner.Core.Events.TaskEvents.TaskStatus;
using JobRunner.Core.Interfaces.Core;
using JobRunner.Core.Interfaces.EntityServices;
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

            // Добавляем настройку для NotifySettings, если нужно
            _taskMock.Setup(x => x.NotifySettings).Returns(new Mock<INotifySettings>().Object);
        }

        private ExecutionScope CreateScope()
        {
            return new ExecutionScope(_taskMock.Object, _loggerMock.Object, _dispatcherMock.Object);
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
                x.DecryptSensitiveArgumentsAsync(_taskMock.Object.ScheduleArguments, It.IsAny<CancellationToken>()),
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
                x.PublishAsync(It.IsAny<TaskStartedEvent>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task UpdateBeforeExecutionAsync_ShouldSetRunningFlagAndUpdate()
        {
            // Arrange
            var metadataMock = new Mock<IJobTaskMetadata>();
            _taskMock.Setup(x => x.JobTaskMetadata).Returns(metadataMock.Object);
            var scope = CreateScope();

            // Act
            await scope.PublishStartedEventAsync(_dispatcherMock.Object);
            await scope.UpdateBeforeExecutionAsync(_storageMock.Object);

            // Assert
            metadataMock.VerifySet(x => x.IsRunning = true, Times.Once);
            metadataMock.VerifySet(x => x.LastRun = scope.StartTime, Times.Once);
            _taskMock.VerifySet(x => x.StartRun = scope.StartTime, Times.Once);
            _storageMock.Verify(x => x.UpdateAsync(_taskMock.Object, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAfterExecutionAsync_WhenSuccessful_ShouldIncrementCounters()
        {
            // Arrange
            var metadataMock = new Mock<IJobTaskMetadata>();
            metadataMock.SetupProperty(x => x.TotalRunCount, 0);
            metadataMock.SetupProperty(x => x.SuccessCount, 0);
            metadataMock.SetupProperty(x => x.ConsecutiveFailures, 3);
            metadataMock.SetupProperty(x => x.IsRunning, true);
            _taskMock.Setup(x => x.JobTaskMetadata).Returns(metadataMock.Object);

            var result = JobExecutionResult.CreateSuccess(12345, DateTime.UtcNow);
            var scope = CreateScope();

            // Act
            await scope.PublishStartedEventAsync(_dispatcherMock.Object);
            await scope.UpdateAfterExecutionAsync(result, _storageMock.Object);

            // Assert
            metadataMock.Object.TotalRunCount.Should().Be(1);
            metadataMock.Object.SuccessCount.Should().Be(1);
            metadataMock.Object.ConsecutiveFailures.Should().Be(0);
            metadataMock.Object.IsRunning.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateAfterExecutionAsync_WhenFailed_ShouldIncrementFailureCount()
        {
            // Arrange
            var metadata = new JobTaskMetadata();  // реальный объект
            _taskMock.Setup(x => x.JobTaskMetadata).Returns(metadata);

            var result = JobExecutionResult.CreateFailure("Test error", DateTime.UtcNow);
            var scope = CreateScope();

            // Act
            await scope.PublishStartedEventAsync(_dispatcherMock.Object);
            await scope.UpdateAfterExecutionAsync(result, _storageMock.Object);

            // Assert
            metadata.TotalRunCount.Should().Be(1);
            metadata.FailureCount.Should().Be(1);
            metadata.ConsecutiveFailures.Should().Be(1);
            metadata.LastError.Should().Be("Test error");
            metadata.IsRunning.Should().BeFalse();
        }

        [Fact]
        public async Task HandleCancellationAsync_ShouldUpdateMetadataAndPublish()
        {
            // Arrange
            var metadata = new JobTaskMetadata { IsRunning = true };
            _taskMock.Setup(x => x.JobTaskMetadata).Returns(metadata);

            var scope = CreateScope();
            var exception = new OperationCanceledException();

            // Act
            await scope.HandleCancellationAsync(exception, _storageMock.Object, _dispatcherMock.Object);

            // Assert
            metadata.IsRunning.Should().BeFalse();
            metadata.FailureCount.Should().Be(1);
            metadata.ConsecutiveFailures.Should().Be(1);
            metadata.LastError.Should().Be("Execution was cancelled");
            _storageMock.Verify(x => x.UpdateAsync(_taskMock.Object, It.IsAny<CancellationToken>()), Times.Once);
            _dispatcherMock.Verify(x => x.PublishAsync(It.IsAny<TaskCompletedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleFailureAsync_ShouldUpdateMetadataAndPublish()
        {
            // Arrange
            var metadata = new JobTaskMetadata { IsRunning = true };
            _taskMock.Setup(x => x.JobTaskMetadata).Returns(metadata);

            var scope = CreateScope();
            var exception = new InvalidOperationException("Something went wrong");

            // Act
            await scope.HandleFailureAsync(exception, _storageMock.Object, _dispatcherMock.Object);

            // Assert
            metadata.IsRunning.Should().BeFalse();
            metadata.FailureCount.Should().Be(1);
            metadata.ConsecutiveFailures.Should().Be(1);
            metadata.LastError.Should().Be("Something went wrong");
            _storageMock.Verify(x => x.UpdateAsync(_taskMock.Object, It.IsAny<CancellationToken>()), Times.Once);
            _dispatcherMock.Verify(x => x.PublishAsync(It.IsAny<TaskCompletedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
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

            // Act & Assert (не должно выбросить исключение)
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