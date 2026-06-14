using System.Threading;
using System.Threading.Tasks;

namespace JobRunner.Core.Interfaces.Detectors.Platform.ProcessExecution
{
    /// <summary>
    /// Платформонезависимый исполнитель процессов
    /// </summary>
    public interface IProcessExecutor
    {
        /// <summary>
        /// Запустить процесс
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="arguments"></param>
        /// <param name="timeoutSeconds"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<IProcessResult> ExecuteAsync(string fileName, string arguments,
                                    int? timeoutSeconds = null,
                                    CancellationToken ct = default);

        /// <summary>
        /// Остановить процесс
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<bool> StopAsync(long processId, CancellationToken ct = default);

        /// <summary>
        /// Проверить, запущен ли процесс
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<bool> IsRunningAsync(long processId, CancellationToken ct = default);
    }
}