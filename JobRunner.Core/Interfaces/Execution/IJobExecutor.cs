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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task ExecuteAsync(Guid taskId, CancellationToken ct);
    }
}
