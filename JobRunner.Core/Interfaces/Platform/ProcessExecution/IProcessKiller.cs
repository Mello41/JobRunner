using System.Threading;
using System.Threading.Tasks;

namespace JobRunner.Core.Interfaces.Platform.ProcessExecution
{
    /// <summary>
    /// Платформозависимая стратегия убийства процессов
    /// </summary>
    public interface IProcessKiller
    {
        /// <summary>
        /// Убить процесс и всех его потомков
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<bool> KillTreeAsync(long processId, CancellationToken ct = default);

        /// <summary>
        /// Отправить сигнал graceful завершения
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<bool> TerminateAsync(long processId, CancellationToken ct = default);

        /// <summary>
        /// Принудительно убить процесс
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<bool> ForceKillAsync(long processId, CancellationToken ct = default);
    }
}
