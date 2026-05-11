using JobRunner.Core.Entities;
using JobRunner.Core.Events.TaskEvents.TaskStatus;
using JobRunner.Core.Results;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JobRunner.Core.Interfaces.Execution
{
    /// <summary>
    /// 
    /// </summary>
    public interface IJobExecutor
    {
        event Func<TaskStartedEvent, Task>? TaskStarted;
        event Func<TaskCompletedEvent, Task>? TaskCompleted;

        /// <summary>
        /// Выполнить задачу
        /// </summary>
        /// <param name="task">Задача для выполнения</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат выполнения</returns>
        /// <remarks>
        /// Если у задачи задан TimeoutSeconds, должен использоваться linked token source
        /// с таймаутом. При превышении таймаута процесс принудительно завершается.
        /// </remarks>
        Task<JobExecutionResult> ExecuteAsync(IJobTask task, CancellationToken ct = default);
    }
}
