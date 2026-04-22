using JobRunner.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobRunner.Core.Repositories
{
    /// <summary>
    /// Репозиторий для работы с задачами (абстракция хранения)
    /// </summary>
    /// <typeparam name="T">Тип сущности задачи, реализующей JobTask</typeparam>
    public interface IJobTaskRepository<T> where T :  IJobTask
    {
        /// <summary>
        /// Получить задачу по идентификатору
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Задача или null, если не найдена</returns>
        Task<T?> GetByIdAsync(Guid id);

        /// <summary>
        /// Получить все задачи
        /// </summary>
        /// <returns>Коллекция всех задач</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Добавить новую задачу
        /// </summary>
        /// <param name="task">Задача для добавления</param>
        /// <returns>Добавленная задача (с сгенерированным Id)</returns>
        Task<T> AddAsync(T task);

        /// <summary>
        /// Обновить существующую задачу
        /// </summary>
        /// <param name="task">Задача с обновлёнными данными</param>
        /// <returns>Задача завершена успешно</returns>
        Task UpdateAsync(T task);

        /// <summary>
        /// Удалить задачу по идентификатору
        /// </summary>
        /// <param name="id">Уникальный идентификатор задачи</param>
        /// <returns>Задача завершена успешно</returns>
        Task DeleteAsync(Guid id);
    }
}
