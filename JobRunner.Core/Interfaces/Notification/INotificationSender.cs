using JobRunner.Core.Entities;
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
    public interface INotificationSender
    {
        /// <summary>
        /// Асинхронно отправляет уведомление о выполнении задачи
        /// </summary>
        /// <param name="task">Выполненная задача (содержит настройки уведомлений, имя, результат)</param>
        /// <param name="isSuccess">
        /// <c>true</c> — задача выполнена успешно (ExitCode == 0);
        /// <c>false</c> — задача завершилась с ошибкой
        /// </param>
        /// <param name="errorMessage">
        /// Сообщение об ошибке (заполняется только если <paramref name="isSuccess"/> == <c>false</c>).
        /// Может быть <c>null</c>.
        /// </param>
        /// <param name="cancellationToken">
        /// Токен для отмены операции отправки (например, при остановке приложения или завершении запроса)
        /// </param>
        /// <returns>Задача, представляющая асинхронную операцию отправки</returns>
        /// <remarks>
        /// <list type="bullet">
        /// <item>Метод не должен выбрасывать исключения — ошибки логируются внутри реализации</item>
        /// <item>Если в настройках задачи <see cref="INotifySettings.NotificationMethods"/> не содержит соответствующий тип, вызов игнорируется</item>
        /// <item>Для Email/Telegram/Webhook рекомендуется учитывать <paramref name="cancellationToken"/> для прерывания долгих операций</item>
        /// </list>
        /// </remarks>
        Task SendAsync(IJobTask task, bool isSuccess, string? 
            errorMessage = null, CancellationToken cancellationToken = default);
    }
}
