using System;

namespace JobRunner.Core.DTO.ScheduleDTO
{
    /// <summary>
    /// Элемент однократного расписания (DTO)
    /// </summary>
    public class OnceScheduleItem
    {
        public long Id { get; set; }
        public long TaskId { get; set; }
        public DateTime ScheduledTime { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? ExecutedAt { get; set; }
        public string? ErrorMessage { get; set; }
        public bool IsEnabled { get; set; }
    }
}
