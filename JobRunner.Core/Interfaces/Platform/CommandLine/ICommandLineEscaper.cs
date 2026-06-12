namespace JobRunner.Core.Interfaces.Platform.CommandLine
{
    /// <summary>
    /// Интерфейс для экранирования аргументов командной строки
    /// Реализации зависят от операционной системы
    /// </summary>
    public interface ICommandLineEscaper
    {
        /// <summary>
        /// Экранирует аргумент для безопасной передачи в командную строку
        /// </summary>
        /// <param name="argument">Аргумент для экранирования</param>
        /// <returns>Экранированный аргумент</returns>
        string EscapeArgument(string argument);

        /// <summary>
        /// Экранирует путь к файлу
        /// </summary>
        /// <param name="path">Путь для экранирования</param>
        /// <returns>Экранированный путь</returns>
        string EscapePath(string path);
    }
}