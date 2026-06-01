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
    public interface ITaskService<T> : ICrudService<T, Guid> where T : class, IJobTask
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
        /// <param name="pid">является необязательным (когда задача не существует)</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <remarks> так как PID существует только когда задача выполняется,
        /// при работе этого метода он необязателен. в основном можно искать по Guid, 
        /// однако поиск по PID будет проходить хорошо, если мы уверены, что задача запущена и не падает</remarks>
        Task<T?> GetStatusAsync(Guid guid, long? pid, CancellationToken cancellationToken = default);
    
    }
}
