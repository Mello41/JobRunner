using JobRunner.Core.Interfaces.Platform.CommandLine;

namespace JobRunner.Core.Interfaces.Platform
{
    /// <summary>
    /// Интерфейс для определения операционной системы
    /// </summary>
    public interface IPlatformDetector
    {
        /// <summary>
        /// Является ли ОС Windows
        /// </summary>
        bool IsWindows { get; }

        /// <summary>
        /// Является ли ОС Linux
        /// </summary>
        bool IsLinux { get; }

        /// <summary>
        /// Является ли ОС macOS
        /// </summary>
        bool IsMacOS { get; }

        /// <summary>
        /// Создает экранировщик для текущей ОС
        /// </summary>
        ICommandLineEscaper CreateEscaper();

        /// <summary>
        /// Возвращает название текущей платформы
        /// </summary>
        string GetCurrentPlatform();
    }
}
