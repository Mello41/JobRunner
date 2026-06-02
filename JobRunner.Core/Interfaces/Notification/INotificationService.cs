using JobRunner.Core.Entities;
using JobRunner.Core.Entities.ValueObjects;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JobRunner.Core.Interfaces.Notification
{
    /// <summary>
    /// Сервис уведомлений (абстракция)
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Отправить уведомление о выполнении задачи (с полным объектом)
        /// </summary>
        Task NotifyAsync(IJobTask task, bool isSuccess, string? errorMessage = null, CancellationToken ct = default);

        /// <summary>
        /// Отправить уведомление о выполнении задачи (по данным из события)
        /// </summary>
        Task NotifyAsync(Guid taskId, string taskName, bool isSuccess, string? errorMessage, INotifySettings? settings, CancellationToken ct = default);

        /// <summary>
        /// Отправить уведомление до выполнения задачи (с полным объектом)
        /// </summary>
        Task NotifyBeforeAsync(IJobTask task, CancellationToken ct = default);

        /// <summary>
        /// Отправить уведомление до выполнения задачи (по данным из события)
        /// </summary>
        Task NotifyBeforeAsync(Guid taskId, string taskName, INotifySettings? settings, CancellationToken ct = default);
    }
}
