using JobRunner.Core.Interfaces.Services.Detectors.PlatformDetector.CommandLine;

namespace JobRunner.Core.Interfaces.Services.Detectors.PlatformDetector
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
