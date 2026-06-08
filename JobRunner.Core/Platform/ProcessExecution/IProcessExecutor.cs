using JobRunner.Core.Results;
using System.Threading;
using System.Threading.Tasks;

namespace JobRunner.Core.Platform.ProcessExecution
{
    public interface IProcessExecutor
    {
        Task<JobExecutionResult> ExecuteAsync(
            string fileName,
            string arguments,
            int? timeoutSeconds = null,
            CancellationToken cancellationToken = default);
    }
}
