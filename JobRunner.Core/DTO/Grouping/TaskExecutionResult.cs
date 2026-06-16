namespace JobRunner.Core.DTO.Grouping
{
    /// <summary>
    /// Результат выполнения отдельной задачи в группе
    /// </summary>
    public class TaskExecutionResult
    {
        public object TaskId { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public bool Success { get; set; }
        public long? DurationMs { get; set; }
        public int? ExitCode { get; set; }
        public string? ErrorMessage { get; set; }
        public string? Output { get; set; }
        public object? PipelineData { get; set; }
        public int OrderInGroup { get; set; }
    }
}
