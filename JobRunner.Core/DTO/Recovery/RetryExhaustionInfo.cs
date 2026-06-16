using System;

namespace JobRunner.Core.DTO.Recovery
{
    public class RetryExhaustionInfo<TId>
    {
        public TId TaskId { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public int MaxAttempts { get; set; }
        public int ConsecutiveFailures { get; set; }
        public string? LastError { get; set; }
        public DateTime? LastErrorTime { get; set; }
        public string Strategy { get; set; } = string.Empty;
        public bool ShouldDisable { get; set; }
    }
}
