using System;

namespace JobRunner.Core.Interfaces.Services.Detectors.PlatformDetector.ProcessExecution
{
    /// <summary>
    /// Результат выполнения процесса
    /// </summary>
    public interface IProcessResult
    {
        /// <summary>PID процесса</summary>
        long? ProcessId { get; }

        /// <summary>Код возврата</summary>
        int? ExitCode { get; }

        /// <summary>Вывод процесса (stdout)</summary>
        string Output { get; }

        /// <summary>Вывод ошибок (stderr)</summary>
        string Error { get; }

        /// <summary>Успешно ли выполнение</summary>
        bool IsSuccess { get; }

        /// <summary>Длительность выполнения</summary>
        TimeSpan Duration { get; }

        /// <summary>Был ли превышен таймаут</summary>
        bool IsTimeout { get; }
    }
}
