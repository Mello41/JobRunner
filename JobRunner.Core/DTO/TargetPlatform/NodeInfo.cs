using JobRunner.Core.Models.Enums;
using System;
using System.Collections.Generic;

namespace JobRunner.Core.DTO.TargetPlatform
{
    /// <summary>
    /// Информация об активном узле
    /// </summary>
    public class NodeInfo
    {
        public string NodeId { get; set; } = string.Empty;
        public string NodeName { get; set; } = string.Empty;
        public string? Group { get; set; }
        public string? IpAddress { get; set; }
        public string? OsPlatform { get; set; }
        public NodeStatus Status { get; set; } = NodeStatus.Offline;
        public DateTime LastHeartbeat { get; set; }
        public DateTime RegisteredAt { get; set; }
        public int CurrentLoad { get; set; }
        public int MaxConcurrentTasks { get; set; } = 1;
        public List<string> Tags { get; set; } = new();
        public Dictionary<string, string> Metadata { get; set; } = new();
    }
}
