using System;

namespace JobRunner.Core.DTO.Results
{
    /// <summary>
    /// Результат отправки уведомления
    /// </summary>
    public class NotificationResult
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
        public string? Channel { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
