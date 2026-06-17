using System;

namespace JobRunner.Core.Models
{
    /// <summary>
    /// Сводка по критическим проблемам
    /// </summary>
    public class CriticalIssuesSummary
    {
        public int StuckTasksCount { get; set; }
        public int MissedSchedulesCount { get; set; }
        public int InfiniteLoopCount { get; set; }
        public int RetryExhaustionCount { get; set; }
        public DateTime CheckedAt { get; set; } = DateTime.UtcNow;
        public bool HasCriticalIssues =>
            StuckTasksCount > 0 ||
            MissedSchedulesCount > 0 ||
            InfiniteLoopCount > 0 ||
            RetryExhaustionCount > 0;
    }
}