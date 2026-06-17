using JobRunner.Core.Interfaces.Entities.JobTaskSettings.NotifySettings;

namespace JobRunner.Core.Interfaces.Events
{
    /// <summary>
    /// Уведомление содержит в себе INotifySettings задачи?
    /// </summary>
    public interface INotifiableEvent
    {
        /// <summary>
        /// Настройки уведомлений у задачи
        /// </summary>
        INotifySettings<TID>? NotifySettings { get; set; }
    }
}
