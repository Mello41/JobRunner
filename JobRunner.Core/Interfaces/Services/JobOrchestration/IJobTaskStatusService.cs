using System;
using System.Threading;
using System.Threading.Tasks;

namespace JobRunner.Core.Interfaces.Services.JobOrchestration
{
    /// <summary>
    /// Управление состояниями задач
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public interface IJobTaskStatusService<TId> where TId : IEquatable<TId>
    {
        /// <summary>
        /// Принудительный немедленный запуск задачи
        /// </summary>
        /// <param name="taskId">Уникальный идентификатор задачи</param>
        /// <returns>true — задача запущена, 
        /// false — задача не найдена</returns>
        /// <remarks>Вызывает IJobScheduler.RunNowAsync</remarks>
        Task<bool> RunNowAsync(TId taskId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Приостановка выполнения задачи по расписанию
        /// смысл:
        /// </summary>
        /// <param name="taskId">Уникальный идентификатор задачи</param>
        /// <returns>true — задача приостановлена, 
        /// false — задача не найдена</returns>
        /// <remarks>
        /// Вызывает IJobScheduler.PauseAsync. 
        /// Может быть возобновлена через ResumeAsync
        /// Чтобы можно было удобно вытащить задачи из 
        /// кеша сервера при перезагрузке и запустить 
        /// все задачи на паузе (и мы поймём, что у этих 
        /// задач не было ошибки и мы их планово остановили)
        /// </remarks>
        Task<bool> PauseAsync(TId taskId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Возобновление выполнения задачи по расписанию
        /// </summary>
        /// <param name="taskId">Уникальный идентификатор задачи</param>
        /// <returns>true — задача возобновлена, 
        /// false — задача не найдена</returns>
        /// <remarks>Вызывает IJobScheduler.ResumeAsync</remarks>
        Task<bool> ResumeAsync(TId taskId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Принудительная остановка выполняющейся задачи
        /// </summary>
        /// <param name="taskId">Уникальный идентификатор задачи</param>
        /// <returns>true — задача остановлена, 
        /// false — задача не найдена или не выполняется</returns>
        /// <remarks>Вызывает IJobScheduler.StopAsync. 
        /// Завершает процесс задачи.</remarks>
        Task<bool> StopAsync(TId taskId, CancellationToken cancellationToken = default);
    }
}