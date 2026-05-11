using JobRunner.Core.Entities.ValueObjects;

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
