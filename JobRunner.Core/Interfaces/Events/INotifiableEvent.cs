using JobRunner.Core.Interfaces.Entities.JobTaskSettings.NotifySettings;
using System;

namespace JobRunner.Core.Interfaces.Events
{
    /// <summary>
    /// Уведомление содержит в себе INotifySettings задачи?
    /// </summary>
    public interface INotifiableEvent<TId> where TId : IEquatable<TId>
    {
        /// <summary>
        /// Настройки уведомлений у задачи
        /// </summary>
        INotifySettings<TId>? NotifySettings { get; set; }
    }
}
