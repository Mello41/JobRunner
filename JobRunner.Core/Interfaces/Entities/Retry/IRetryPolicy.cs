using JobRunner.Core.Interfaces.Entities.JobTaskSettings;

namespace JobRunner.Core.Interfaces.Entities.Retry
{
    /// <summary>
    /// Стратегия повторных попыток — 
    /// реализует алгоритмы расчета задержек и определения необходимости повтора
    /// </summary>
    public interface IRetryPolicy
    {
        /// <summary>
        /// Название стратегии (для логирования и UI)
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Рассчитывает задержку перед следующей попыткой
        /// </summary>
        /// <param name="attemptNumber">Номер текущей попытки (1 - первая, 2 - первый повтор и т.д.)</param>
        /// <param name="settings">Настройки повторных попыток</param>
        /// <returns>Задержка в миллисекундах</returns>
        int GetDelayMilliseconds(int attemptNumber, IRetrySettings settings);

        /// <summary>
        /// Определяет, нужно ли повторять выполнение при данной ошибке
        /// </summary>
        /// <param name="errorMessage">Сообщение об ошибке</param>
        /// <param name="exitCode">Код завершения процесса</param>
        /// <param name="isTimeout">Был ли превышен таймаут</param>
        /// <param name="currentAttempt">Текущий номер попытки</param>
        /// <param name="settings">Настройки повторных попыток</param>
        /// <returns>true — стоит повторить, false — окончательная ошибка</returns>
        bool ShouldRetry(string? errorMessage, int? exitCode, bool isTimeout,
                        int currentAttempt, IRetrySettings settings);
    }
}
