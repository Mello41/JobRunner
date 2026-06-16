using System;
using System.Threading;
using System.Threading.Tasks;
using JobRunner.Core.Models.Enums;

namespace JobRunner.Core.Interfaces.Core
{
    /// <summary>
    /// Unit of Work (UoW) — это паттерн, который 
    /// отслеживает все изменения, сделанные в 
    /// течение одной бизнес-операции, и фиксирует 
    /// их в базе данных одним атомарным действием 
    /// (всё или ничего)
    /// в этой библиотеке поможет след. образом:
    /// Все изменения копятся в памяти, а в БД отправляются 
    /// ОДНИМ РАЗОМ в конце. Если ошибка → отмена 
    /// ВСЕХ изменений, БД остаётся целостной
    /// </summary>
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// Начать транзакцию (опционально)
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task BeginTransactionAsync(CancellationToken ct = default);

        /// <summary>
        /// Сохранить ВСЕ изменения (без коммита транзакции)
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<int> SaveChangesAsync(CancellationToken ct = default);

        /// <summary>
        /// Сохранить изменения и закоммитить транзакцию
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<int> CommitAsync(CancellationToken ct = default);

        /// <summary>
        /// Откатить транзакцию
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task RollbackAsync(CancellationToken ct = default);

        /// <summary>
        /// Получить сервис для работы с сущностью (ваш ICrudService)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <returns></returns>
        ICrudService<T, TKey> GetService<T, TKey>() where T : class;

        /// <summary>
        /// Регистрирует изменения в UoW (без немедленного сохранения)
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="state"></param>
        void TrackChanges(object entity, EntityState state);
    }
}