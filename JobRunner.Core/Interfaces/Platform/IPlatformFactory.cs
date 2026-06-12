using JobRunner.Core.Interfaces.Platform.CommandLine;

namespace JobRunner.Core.Interfaces.Platform
{
    /// <summary>
    /// Фабрика для создания платформозависимых сервисов
    /// </summary>
    public interface IPlatformFactory
    {
        /// <summary>
        /// Создает экранировщик аргументов
        /// </summary>
        ICommandLineEscaper CreateCommandLineEscaper();

        /// <summary>
        /// Создает исполнитель процессов
        /// </summary>
        IProcessRunner CreateProcessRunner();

        /// <summary>
        /// Детектор платформы
        /// </summary>
        IPlatformDetector Detector { get; }
    }
}
