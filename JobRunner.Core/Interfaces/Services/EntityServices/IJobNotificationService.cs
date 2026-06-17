using JobRunner.Core.Interfaces.Entities;
using JobRunner.Core.Interfaces.Entities.JobTaskSettings.NotifySettings;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JobRunner.Core.Interfaces.Services.EntityServices
{
    /// <summary>
    /// Сервис уведомлений (абстракция)
    /// </summary>
    public interface IJobNotificationService<TID> where TID : IEquatable<TID>
    {
        /// <summary>
        /// Отправить уведомление о выполнении задачи (с полным объектом)
        /// </summary>
        Task NotifyAsync(IJobTask<TID> task, bool isSuccess, string? errorMessage = null, CancellationToken ct = default);

        /// <summary>
        /// Отправить уведомление о выполнении задачи (по данным из события)
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="taskName"></param>
        /// <param name="isSuccess"></param>
        /// <param name="errorMessage"></param>
        /// <param name="settings"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task NotifyAsync(TID taskId, string taskName, 
            bool isSuccess, string? errorMessage, 
            INotifySettings<TID>? settings, CancellationToken ct = default);

        /// <summary>
        /// Отправить уведомление до выполнения задачи (с полным объектом)
        /// </summary>
        Task NotifyBeforeAsync(IJobTask<TID> task, CancellationToken ct = default);

        /// <summary>
        /// Отправить уведомление до выполнения задачи (по данным из события)
        /// </summary>
        Task NotifyBeforeAsync(TID taskId, string taskName, INotifySettings<TID>? settings, CancellationToken ct = default);
    }
}
