using JobRunner.Core.Interfaces.Core;
using JobRunner.Core.Interfaces.Entities.JobTaskSettings.NotifySettings;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JobRunner.Core.Interfaces.Services.EntityServices.JobTaskSettingsServices
{
    /// <summary>
    /// Сервис для управления настройками уведомлений задач
    /// </summary>
    /// <typeparam name="T">Тип сущности настроек (должен реализовывать INotifySettings)</typeparam>
    /// <typeparam name="TKey">Тип идентификатора (long, Guid, string)</typeparam>
    public interface IJobNotifySettingsService<T, TKey> : ICrudService<T, TKey>
                            where T : class, INotifySettings<TKey>
                            where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Получить настройки уведомлений по ID задачи
        /// </summary>
        /// <param name="taskId">ID задачи</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Настройки уведомлений или null</returns>
        Task<T?> GetByTaskIdAsync(TKey taskId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Проверить, существуют ли настройки для задачи
        /// </summary>
        /// <param name="taskId">ID задачи</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>true если настройки существуют</returns>
        Task<bool> ExistsForTaskAsync(TKey taskId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Удалить настройки уведомлений по ID задачи
        /// </summary>
        /// <param name="taskId">ID задачи</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>true если удаление успешно</returns>
        Task<bool> DeleteByTaskIdAsync(TKey taskId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Создать или обновить настройки (upsert)
        /// </summary>
        /// <param name="taskId">ID задачи</param>
        /// <param name="settings">Настройки</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Сохраненные настройки</returns>
        Task<T> UpsertAsync(TKey taskId, T settings, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить настройки по умолчанию для задачи
        /// </summary>
        /// <param name="taskId">ID задачи</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Настройки по умолчанию</returns>
        Task<T> GetDefaultSettingsAsync(TKey taskId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Клонировать настройки от другой задачи
        /// </summary>
        /// <param name="sourceTaskId">ID задачи-источника</param>
        /// <param name="targetTaskId">ID задачи-цели</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Клонированные настройки</returns>
        Task<T> CloneFromTaskAsync(TKey sourceTaskId, TKey targetTaskId, CancellationToken cancellationToken = default);
    }
}
