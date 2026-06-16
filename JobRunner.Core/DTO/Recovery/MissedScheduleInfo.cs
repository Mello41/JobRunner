using System;

namespace JobRunner.Core.DTO.Recovery
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public class MissedScheduleInfo<TId>
    {
        public TId TaskId { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public DateTime ScheduledTime { get; set; }
        public DateTime DetectedAt { get; set; }
        public TimeSpan MissedDuration { get; set; }
        public int ConsecutiveMisses { get; set; }
        public string ScheduleDescription { get; set; } = string.Empty;
        public bool CanExecuteNow { get; set; }
    }
}
