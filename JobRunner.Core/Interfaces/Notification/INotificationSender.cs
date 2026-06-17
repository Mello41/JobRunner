using JobRunner.Core.Interfaces.Entities;
using JobRunner.Core.Interfaces.Entities.JobTaskSettings.NotifySettings;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JobRunner.Core.Interfaces.Notification
{
    /// <summary>
    /// Отвечает за отправку уведомлений через конкретный канал 
    /// (Email, Telegram, Webhook, Popup и т.д.)
    /// </summary>
    /// <remarks>
    /// Каждая реализация отвечает за один способ уведомления.
    /// Регистрируются в DI как scoped или transient сервисы.
    /// </remarks>
    public interface INotificationSender<TId> where TId : IEquatable<TId>
    {
        /// <summary>
        /// Асинхронно отправляет уведомление о выполнении задачи (с полным объектом)
        /// </summary>
        /// <param name="task"></param>
        /// <param name="isSuccess"></param>
        /// <param name="errorMessage"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SendAsync(IJobTask<TId> task, bool isSuccess, 
            string? errorMessage = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Асинхронно отправляет уведомление о выполнении задачи (по данным из события)
        /// </summary>
        /// <param name="taskId">ID задачи</param>
        /// <param name="taskName">Название задачи</param>
        /// <param name="isSuccess">Успешно ли выполнена</param>
        /// <param name="errorMessage">Сообщение об ошибке</param>
        /// <param name="settings">Настройки уведомлений</param>
        /// <param name="cancellationToken">Токен отмены</param>
        Task SendAsync(TId taskId, string taskName, bool isSuccess, 
            string? errorMessage, INotifySettings<TId> settings, 
            CancellationToken cancellationToken = default);
    }
}