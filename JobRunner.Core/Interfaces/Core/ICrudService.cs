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
        Task<T?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Получить все сущности
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Создать сущность
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// PUT (обновить) значения сущности по id
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync(T entity, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Удалить сущность по id (guid)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(TKey id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить общее количество сущностей
        /// </summary>
        Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить количество сущностей по условию
        /// </summary>
        /// <param name="filter">Фильтр (опционально)</param>
        Task<int> GetTotalCountAsync(System.Linq.Expressions.Expression<Func<T, bool>>? filter = null,
                                    CancellationToken cancellationToken = default);
    }
}
