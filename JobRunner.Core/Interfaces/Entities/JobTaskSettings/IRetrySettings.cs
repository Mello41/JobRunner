using JobRunner.Core.Interfaces.Entities.Retry;
using System.Collections.Generic;

namespace JobRunner.Core.Interfaces.Entities.JobTaskSettings
{
    /// <summary>
    /// Настройки политики повторных попыток при сбое выполнения задачи
    /// </summary>
    public interface IRetrySettings
    {
        /// <summary>
        /// Максимальное количество попыток выполнения задачи (включая первую)
        /// </summary>
        int MaxAttempts { get; set; }

        /// <summary>
        /// Стратегия расчета задержки между повторными попытками
        /// </summary>
        IRetryPolicyStrategy RetryPolicy { get; set; }

        /// <summary>
        /// Начальная задержка перед первой повторной попыткой (в секундах)
        /// </summary>
        int InitialDelaySeconds { get; set; }

        /// <summary>
        /// Максимальная задержка между попытками (в секундах)
        /// </summary>
        int? MaxDelaySeconds { get; set; }

        /// <summary>
        /// Повторять попытки при превышении таймаута задачи
        /// </summary>
        /// <remarks>
        /// true — таймаут считается ошибкой, достойной повторной попытки.
        /// false — при таймауте задача считается окончательно проваленной.
        /// </remarks>
        bool RetryOnTimeout { get; set; }

        /// <summary>
        /// Повторять попытки при любой ошибке (если true, то RetryableErrorMessages игнорируется)
        /// </summary>
        bool RetryOnAnyError { get; set; }

        /// <summary>
        /// Список фрагментов сообщений об ошибках, при которых стоит повторять попытку
        /// </summary>
        IReadOnlyList<string> RetryableErrorMessages { get; }

        /// <summary>
        /// Повторять попытки при определенных кодах завершения процесса (ExitCode)
        /// </summary>
        IReadOnlyList<int> RetryableExitCodes { get; }
    }
}
