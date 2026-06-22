using JobRunner.Core.DTO.Grouping;
using JobRunner.Core.Interfaces.Entities.JobTaskSettings.NotifySettings.GroupSettings;
using System;
using System.Collections.Generic;

namespace JobRunner.Core.Interfaces.Events.GroupEvents
{
    /// <summary>
    /// Событие: группа задач завершена
    /// </summary>
    public interface IGroupCompletedEvent<TId> where TId : IEquatable<TId>
    {
        TId TagId { get; set; }
        string TagName { get; set; }
        DateTime StartTime { get; set; }
        DateTime EndTime { get; set; }
        int TotalTasks { get; set; }
        int SuccessCount { get; set; }
        int FailureCount { get; set; }
        TimeSpan Duration { get; set; }
        bool Success { get; set; }
        string? ErrorMessage { get; set; }

        /// <summary>
        /// Настройки уведомлений для группы
        /// </summary>
        IGroupNotifySettings<TId>? NotifySettings { get; set; }

        /// <summary>
        /// Результаты выполнения каждой задачи
        /// </summary>
        IReadOnlyList<TaskExecutionResult> TaskResults { get; set; }
    }
}