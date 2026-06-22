using JobRunner.Core.Interfaces.Entities.JobTaskSettings.NotifySettings.GroupSettings;
using JobRunner.Core.Models.Enums.ExecutionEnums;
using System;
using System.Collections.Generic;

namespace JobRunner.Core.Interfaces.Events.GroupEvents
{
    /// <summary>
    /// Событие: группа задач запущена
    /// </summary>
    public interface IGroupStartedEvent<TId> where TId : IEquatable<TId>
    {
        TId TagId { get; set; }
        string TagName { get; set; }
        int TotalTasks { get; set; }
        DateTime StartTime { get; set; }
        ExecutionGroupMode Mode { get; set; }

        /// <summary>
        /// Настройки уведомлений для группы
        /// </summary>
        IGroupNotifySettings<TId>? NotifySettings { get; set; }

        /// <summary>
        /// Список задач в группе
        /// </summary>
        IReadOnlyList<TId> TaskIds { get; set; }
    }
}
