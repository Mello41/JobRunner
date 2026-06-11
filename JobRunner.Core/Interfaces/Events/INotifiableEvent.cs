using JobRunner.Core.Interfaces.Entities.JobTaskSettings;

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
        INotifySettings? NotifySettings { get; set; }
    }
}
