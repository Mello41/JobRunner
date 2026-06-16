using System.Threading;
using System.Threading.Tasks;

namespace JobRunner.Core.Interfaces.Scheduler
{
    /// <summary>
    /// Слушатель событий выполнения задач в планировщике
    /// </summary>
    public interface IJobExecutionListener
    {
        /// <summary>
        /// Вызывается перед выполнением задачи
        /// </summary>
        /// <param name="jobKey"></param>
        /// <param name="jobData"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task OnJobExecutingAsync(object jobKey, object? jobData, CancellationToken ct = default);

        /// <summary>
        /// Вызывается после выполнения задачи
        /// </summary>
        /// <param name="jobKey"></param>
        /// <param name="jobData"></param>
        /// <param name="success"></param>
        /// <param name="errorMessage"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task OnJobExecutedAsync(object jobKey, object? jobData, bool success, string? errorMessage = null, CancellationToken ct = default);

        /// <summary>
        /// Вызывается когда выполнение задачи отменено (veto)
        /// </summary>
        /// <param name="jobKey"></param>
        /// <param name="jobData"></param>
        /// <param name="reason"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task OnJobExecutionVetoedAsync(object jobKey, object? jobData, string? reason = null, CancellationToken ct = default);
    }
}
