using JobRunner.Core.Entities;
using JobRunner.Core.Interfaces.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JobRunner.Core.Interfaces.EntityServices
{
    /// <summary>
    /// Набор методов для работы с задачей JobTask --> БД
    /// (операции только с БД)
    /// </summary>
    /// <typeparam name="T">Тип задачи, реализующий IJobTask</typeparam>
    public interface ITaskService<T, TId> : ICrudService<T, TId>
                                    where T : class, IJobTask<TId>
                                    where TId : IEquatable<TId>
    {
        /// <summary>
        /// Получение задачи по идентификатору процесса
        /// </summary>
        /// <param name="pid">Идентификатор процесса (PID)</param>
        /// <returns>Задача или null, если не найдена</returns>
        /// <remarks>Важный ньюанс - PID существует только когда задача выполняется</remarks>
        Task<T?> GetByPIDAsync(long pid, CancellationToken cancellationToken = default);

        /// <summary>
        /// Мето для получения статуса задачи (выполнена/включена/завершена с ошибкой и тд)
        /// </summary>
        /// <param name="guid">Идентификатор процесса (основной)</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<T?> GetStatusAsync(TId guid, CancellationToken cancellationToken = default);

        /// <summary>
        /// Очистить все аргументы задачи
        /// </summary>
        /// <param name="taskId">Идентификатор задачи</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>true — если аргументы очищены, false — задача не найдена</returns>
        Task<bool> ClearAllArgumentsAsync(TId taskId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Обновить значение аргумента задачи
        /// </summary>
        /// <param name="taskId">Идентификатор задачи</param>
        /// <param name="key">Ключ аргумента</param>
        /// <param name="value">Новое значение</param>
        /// <param name="isEncrypted">Зашифровано ли значение</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>true — если аргумент обновлён, false — задача или аргумент не найдены</returns>
        Task<bool> UpdateArgumentValueAsync(TId taskId, string key, object value,
                                            bool isEncrypted = false, CancellationToken cancellationToken = default);
    }
}