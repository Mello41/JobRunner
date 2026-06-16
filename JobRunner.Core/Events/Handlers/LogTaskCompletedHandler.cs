using JobRunner.Core.Events.TaskEvents.TaskStatus;
using JobRunner.Core.Interfaces.Events.DomainEvent;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JobRunner.Core.Events.Handlers
{
    /// <summary>
    /// Логирование завершения задачи
    /// </summary>
    public class LogTaskCompletedHandler<TId> : IDomainEventHandler<TaskCompletedEvent<TId>>
                                where TId : IEquatable<TId>
    {
        private readonly ILogger<LogTaskCompletedHandler<TId>> _logger;

        public LogTaskCompletedHandler(ILogger<LogTaskCompletedHandler<TId>> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Обрабатывает событие завершения задачи и записывает результат в лог
        /// </summary>
        /// <param name="evt">Событие завершения задачи, содержащее информацию о результате выполнения</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Завершенная задача (Task)</returns>
        public Task HandleAsync(TaskCompletedEvent<TId> @evt, CancellationToken cancellationToken)
        {
            if (@evt.Success)
            {
                _logger.LogInformation(
                    "Task {TaskName} (Id: {TaskId}) completed successfully in {DurationMs} ms",
                    @evt.TaskName, @evt.TaskId, @evt.DurationMs);
            }
            else
            {
                _logger.LogError(
                    "Task {TaskName} (Id: {TaskId}) failed: {ErrorMessage}",
                    @evt.TaskName, @evt.TaskId, @evt.ErrorMessage);
            }

            return Task.CompletedTask;
        }
    }
}
