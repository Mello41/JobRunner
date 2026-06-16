using JobRunner.Core.Models.Enums;
using System;

namespace JobRunner.Core.DTO.Recovery
{
    public class InfiniteLoopSuspicion<TId>
    {
        public TId TaskId { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public long? TaskPID { get; set; }
        public DateTime StartedAt { get; set; }
        public TimeSpan CurrentDuration { get; set; }
        public long? AverageDurationMs { get; set; }
        public double ExceedsAverageBy { get; set; }
        public InfiniteLoopSeverity Severity { get; set; }
    }
}
