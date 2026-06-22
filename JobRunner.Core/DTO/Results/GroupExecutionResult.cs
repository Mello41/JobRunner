using JobRunner.Core.DTO.Grouping;
using System;
using System.Collections.Generic;

namespace JobRunner.Core.DTO.Results
{
    /// <summary>
    /// Результат выполнения группы задач
    /// </summary>
    public class GroupExecutionResult
    {
        public string GroupName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool Success { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int FailedTasks { get; set; }
        public TimeSpan? TotalDuration { get; set; }
        public List<TaskExecutionResult> TaskResults { get; set; } = new();
        public string? ErrorMessage { get; set; }
    }
}
