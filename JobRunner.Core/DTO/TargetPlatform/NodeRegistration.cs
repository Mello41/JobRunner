using System.Collections.Generic;

namespace JobRunner.Core.DTO.TargetPlatform
{
    /// <summary>
    /// Регистрационная информация об узле-исполнителе
    /// </summary>
    public class NodeRegistration
    {
        /// <summary>
        /// Уникальный ID узла
        /// </summary>
        public string NodeId { get; set; } = string.Empty;

        /// <summary>
        /// Отображаемое имя узла
        /// </summary>
        public string NodeName { get; set; } = string.Empty;

        /// <summary>
        /// Группа узла (например, "build-agents", "test-agents")
        /// </summary>
        public string? Group { get; set; }

        /// <summary>
        /// IP адрес узла
        /// </summary>
        public string? IpAddress { get; set; }

        /// <summary>
        /// Операционная система (Windows, Linux, macOS)
        /// </summary>
        public string? OsPlatform { get; set; }

        /// <summary>
        /// Максимальное количество одновременно выполняемых задач
        /// </summary>
        public int MaxConcurrentTasks { get; set; } = 1;

        /// <summary>
        /// Метки узла (например, "high-memory", "gpu")
        /// </summary>
        public List<string> Tags { get; set; } = new();

        /// <summary>
        /// Дополнительные метаданные
        /// </summary>
        public Dictionary<string, string> Metadata { get; set; } = new();
    }
}
