using JobRunner.Core.Interfaces.Entities.Export;
using System;

namespace JobRunner.Core.DefaultImplementations
{
    public class JobTaskBasicInfoExample<TId> : IJobTaskBasicInfo<TId> where TId : IEquatable<TId>
    {
        public TId Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ExecutionPath { get; set; } = string.Empty;
        public int? TimeoutSeconds { get; set; }
        public bool AllowConcurrentExecution { get; set; }
        public bool IsAsyncExecution { get; set; }
        public DateTime StartRun { get; set; }
        public DateTime EndRun { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsRunning { get; set; }
        public DateTime? LastRun { get; set; }
        public DateTime? NextRun { get; set; }
        public int TotalRunCount { get; set; }
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
    }
}
