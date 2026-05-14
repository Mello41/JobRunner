using JobRunner.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JobRunner.Core.Interfaces.Core
{
    /// <summary>
    /// Базовый интерфейс для сущностей
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface ICrudService<T, in TKey> where T : class
    {
        /// <summary>
        /// Получить сущность по id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [RequiresPermission("read")]
        Task<T?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Получить все сущности
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [RequiresPermission("read")]
        Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Создать сущность
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [RequiresAdmin]
        Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// PUT (обновить) значения сущности по id
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [RequiresAdmin]
        Task<bool> UpdateAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Удалить сущность по id (guid)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [RequiresAdmin]
        Task<bool> DeleteAsync(TKey id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить общее количество сущностей
        /// </summary>
        [RequiresPermission("read")]
        Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить количество сущностей по условию
        /// </summary>
        /// <param name="filter">Фильтр (опционально)</param>
        [RequiresPermission("read")]
        Task<int> GetTotalCountAsync(System.Linq.Expressions.Expression<Func<T, bool>>? filter = null,
                                    CancellationToken cancellationToken = default);
    }
}
