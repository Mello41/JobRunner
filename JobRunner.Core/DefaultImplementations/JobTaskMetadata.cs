using JobRunner.Core.Interfaces.Entities.JobTaskSettings;
using System;

namespace JobRunner.Core.DefaultImplementations
{
    public class JobTaskMetadataExample : IJobTaskMetadata
    {
        public long? TaskPID { get; set; }
        public DateTime? LastRun { get; set; }
        public DateTime? NextRun { get; set; }
        public long? LastDurationMs { get; set; }
        public string Version { get; set; }
        public int TotalRunCount { get; set; }
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public int ConsecutiveFailures { get; set; }
        public string? LastError { get; set; }
        public DateTime? LastErrorTime { get; set; }
        public bool IsRunning { get; set; }
        public bool IsCompleted { get; set; }

        public void ResetStats()
        {
            TotalRunCount = 0;
            SuccessCount = 0;
            FailureCount = 0;
            ConsecutiveFailures = 0;
            LastError = null;
            LastErrorTime = null;
            LastRun = null;
            LastDurationMs = null;
            IsRunning = false;
            TaskPID = null;
            NextRun = null;   
            IsCompleted = false;   
        }
    }
}
