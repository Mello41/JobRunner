using JobRunner.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobRunner.Core.Interfaces
{
    /// <summary>
    /// Набор методов для работы с задачей --> ханилище задач (операции с БД)
    /// </summary>
    /// <typeparam name="T">Тип задачи, реализующий IJobTask</typeparam>
    public interface ITaskStorage<T> where T : IJobTask
    {
        /// <summary>
        /// Получение задачи по уникальному идентификатору
        /// </summary>
        /// <param name="id">Уникальный идентификатор задачи (Guid)</param>
        /// <returns>Задача или null, если не найдена</returns>
        Task<T?> GetByIdAsync(Guid id);

        /// <summary>
        /// Получение задачи по идентификатору процесса
        /// </summary>
        /// <param name="pid">Идентификатор процесса (PID)</param>
        /// <returns>Задача или null, если не найдена</returns>
        /// <remarks>Важный ньюанс - PID существует только когда задача выполняется</remarks>
        Task<T?> GetByPIDAsync(long pid);

        /// <summary>
        /// Получение всех задач
        /// </summary>
        /// <returns>Коллекция всех задач, отсортированных по Id</returns>
        Task<IReadOnlyList<T>> GetAllAsync();

        /// <summary>
        /// Создание новой задачи
        /// </summary>
        /// <param name="task">Задача для создания (Id может быть пустым)</param>
        /// <returns>Созданная задача с присвоенным Id</returns>
        /// <remarks>Если task.Id == Guid.Empty, генерация нового Guid</remarks>
        Task<T> CreateAsync(T task);

        /// <summary>
        /// Обновление существующей задачи
        /// </summary>
        /// <param name="task">Задача с обновлёнными данными</param>
        /// <returns>true — обновление успешно, 
        /// false — задача не найдена</returns>
        Task<bool> UpdateAsync(T task);

        /// <summary>
        /// Удаление задачи по идентификатору
        /// </summary>
        /// <param name="id">Уникальный идентификатор задачи (Guid)</param>
        /// <returns>true — удаление успешно, 
        /// false — задача не найдена</returns>
        Task<bool> DeleteAsync(Guid id);
    }
}
