using JobRunner.Core.Interfaces.Platform.ProcessExecution;
using System.Threading;
using System.Threading.Tasks;

namespace JobRunner.Core.Interfaces.Platform
{
    /// <summary>
    /// Платформонезависимый исполнитель процессов
    /// </summary>
    public interface IProcessRunner
    {
        /// <summary>
        /// Запускает процесс и ожидает его завершения
        /// </summary>
        /// <param name="fileName">Исполняемый файл</param>
        /// <param name="arguments">Аргументы командной строки</param>
        /// <param name="timeoutSeconds">Таймаут в секундах (null = бесконечно)</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат выполнения процесса</returns>
        Task<IProcessResult> RunAsync(
            string fileName,
            string arguments,
            int? timeoutSeconds = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Принудительно завершает процесс и всех его потомков
        /// </summary>
        /// <param name="processId">PID процесса</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>true если процесс был завершен</returns>
        Task<bool> KillTreeAsync(long processId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Проверяет, запущен ли процесс
        /// </summary>
        /// <param name="processId">PID процесса</param>
        /// <returns>true если процесс запущен</returns>
        Task<bool> IsRunningAsync(long processId);
    }
}
