using System;

namespace JobRunner.Core.DTO.Recovery
{
    /// <summary>
    /// Информация о "зависшей" задаче
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public class StuckTaskInfo<TId>
    {
        public TId TaskId { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public long? Pid { get; set; }
        public DateTime StartedAt { get; set; }
        public TimeSpan StuckDuration => DateTime.UtcNow - StartedAt;
        public bool ProcessExists { get; set; }
    }
}
