using System;

namespace JobRunner.Core.DTO
{
    /// <summary>
    /// DTO для уведомления UI
    /// </summary>
    public class UiNotification
    {
        public EventType TypeNotify { get; set; } = string.Empty; 
        public Guid TaskId { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public object? Data { get; set; }
    }
}