using JobRunner.Core.Entities;
using JobRunner.Core.Events.TaskEvents.TaskStatus;
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
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<JobExecutionResult> ExecuteAsync(IJobTask task, CancellationToken ct = default);
    }
}
