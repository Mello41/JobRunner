using JobRunner.Core.Events.TaskEvents.TaskStatus;
using JobRunner.Core.Interfaces.Entities;
using JobRunner.Core.Results;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JobRunner.Core.Interfaces.Execution
{
    /// <summary>
    /// Исполнитель задачи — отвечает за низкоуровневый запуск внешнего процесса.
    /// </summary>
    /// <typeparam name="TId">Тип идентификатора задачи</typeparam>
    public interface IJobExecutor<TId> where TId : IEquatable<TId>
    {
        event Func<TaskStartedEvent<TId>, Task>? TaskStarted;
        event Func<TaskCompletedEvent<TId>, Task>? TaskCompleted;

        /// <summary>
        /// Выполнить задачу
        /// </summary>
        /// <param name="task">Задача для выполнения</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Результат выполнения</returns>
        Task<JobExecutionResult> ExecuteAsync(IJobTask<TId> task, CancellationToken ct = default);
    }
}
