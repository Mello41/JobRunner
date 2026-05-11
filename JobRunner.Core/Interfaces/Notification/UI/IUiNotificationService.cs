using JobRunner.Core.DTO;
using System.Threading;
using System.Threading.Tasks;

namespace JobRunner.Core.Interfaces.Notification.UI
{
    /// <summary>
    /// Сервис для отправки уведомлений в UI (абстракция)
    /// Реализация будет в UI проекте (WPF/Blazor/Console)
    /// </summary>
    public interface IUiNotificationService
    {
        /// <summary>
        /// Уведомить UI о событии задачи
        /// </summary>
        Task NotifyAsync(UiNotification notification, CancellationToken ct = default);
    }
}