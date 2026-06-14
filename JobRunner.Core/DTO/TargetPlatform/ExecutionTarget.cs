using JobRunner.Core.Models.Enums;

namespace JobRunner.Core.DTO.TargetPlatform
{
    /// <summary>
    /// Цель выполнения задачи (на каком компьютере/узле выполнять)
    /// </summary>
    public class ExecutionTarget
    {
        /// <summary>
        /// Режим выполнения
        /// </summary>
        public ExecutionMode Mode { get; set; } = ExecutionMode.Server;

        /// <summary>
        /// ID узла (для Mode = SpecificNode)
        /// </summary>
        public string? NodeId { get; set; }

        /// <summary>
        /// Группа узлов (для Mode = AnyNodeInGroup, RoundRobinInGroup)
        /// </summary>
        public string? NodeGroup { get; set; }

        /// <summary>
        /// Таймаут ожидания ответа от узла (секунды)
        /// </summary>
        public int? NodeTimeoutSeconds { get; set; }

        /// <summary>
        /// Максимальное количество перенаправлений при недоступности узла
        /// </summary>
        public int MaxFailoverAttempts { get; set; } = 1;

        /// <summary>
        /// Создать целевой узел для выполнения на сервере
        /// </summary>
        public static ExecutionTarget Server() => new() { Mode = ExecutionMode.Server };

        /// <summary>
        /// Создать целевой узел для выполнения на конкретном узле
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public static ExecutionTarget SpecificNode(string nodeId) => new()
        {
            Mode = ExecutionMode.SpecificNode,
            NodeId = nodeId
        };

        /// <summary>
        /// Создать целевой узел для выполнения на любом узле из группы
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public static ExecutionTarget AnyNodeInGroup(string group) => new()
        {
            Mode = ExecutionMode.AnyNode,
            NodeGroup = group
        };
    }
}
