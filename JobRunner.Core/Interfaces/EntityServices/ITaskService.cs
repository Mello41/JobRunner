using JobRunner.Core.Entities;
using JobRunner.Core.Interfaces.Core;
using System;
using System.Threading.Tasks;

namespace JobRunner.Core.Interfaces.EntityServices
{
    /// <summary>
    /// Набор методов для работы с задачей JobTask --> БД
    /// (операции только с БД)
    /// </summary>
    /// <typeparam name="T">Тип задачи, реализующий IJobTask</typeparam>
    public interface ITaskService<T> : ICrudService<T, Guid> where T : class, IJobTask
    {
        /// <summary>
        /// Получение задачи по идентификатору процесса
        /// </summary>
        /// <param name="pid">Идентификатор процесса (PID)</param>
        /// <returns>Задача или null, если не найдена</returns>
        /// <remarks>Важный ньюанс - PID существует только когда задача выполняется</remarks>
        Task<T?> GetByPIDAsync(long pid);

    }
}
