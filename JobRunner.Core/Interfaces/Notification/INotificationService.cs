using JobRunner.Core.Entities;
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
        /// Отправить уведомление о выполнении задачи
        /// </summary>
        /// <param name="task">Задача</param>
        /// <param name="isSuccess">Успешно ли выполнена</param>
        /// <param name="errorMessage">Сообщение об ошибке (если есть)</param>
        Task NotifyAsync(IJobTask task, bool isSuccess, string? 
                            errorMessage = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Отправить уведомление до выполнения задачи
        /// </summary>
        Task NotifyBeforeAsync(IJobTask task, CancellationToken cancellationToken = default);
    }
}
