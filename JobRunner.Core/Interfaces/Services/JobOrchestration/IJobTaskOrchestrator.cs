using JobRunner.Core.Interfaces.Entities;
using JobRunner.Core.Results;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JobRunner.Core.Interfaces.Services.JobOrchestration
{
    /// <summary>
    /// Оркестратор задач (координация хранилища и планировщика)
    /// </summary>
    /// <typeparam name="TTask">Тип задачи</typeparam>
    /// <typeparam name="TId">Тип идентификатора</typeparam>
    public interface IJobTaskOrchestrator<TTask, TId> :
                                IJobTaskCriticalService<TId>,
                                IJobTaskGroupService<TTask, TId>,
                                IJobTaskStatusService<TId>
                                where TTask : class, IJobTask<TId>
                                where TId : IEquatable<TId>
    {
        /// <summary>
        /// Создание задачи и её регистрация в планировщике
        /// </summary>
        /// <param name="task">Задача для создания (Id может пусто быть)</param>
        /// <param name="cancellationToken"></param>
        /// <returns>T - созданная задача с присвоенным значением Id</returns>
        Task<TTask> CreateAndScheduleAsync(TTask task, CancellationToken cancellationToken = default);

        /// <summary>
        /// Обновление задачи и перерегистрация в планировщике
        /// </summary>
        /// <param name="task">Задача с обновлёнными данными</param>
        /// <returns>true — обновление успешно, 
        /// false — задача не найдена</returns>
        Task<bool> UpdateAndRescheduleAsync(TTask task, CancellationToken cancellationToken = default);

        /// <summary>
        /// Удаление задачи и её удаление из планировщика
        /// </summary>
        /// <param name="taskId">Уникальный идентификатор задачи</param>
        /// <returns> true — удаление успешно,
        /// false — задача не найдена </returns>
        Task<bool> DeleteAndUnscheduleAsync(TId taskId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Загрузить все задачи из БД и восстановить их в планировщике
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task RestoreAllTasksAsync(CancellationToken ct = default);

        /// <summary>
        /// Получение задачи по идентификатору
        /// </summary>
        /// <param name="taskId">Уникальный идентификатор задачи</param>
        /// <returns>Задача или null, если не найдена</returns>
        Task<TTask?> GetTaskAsync(TId taskId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получение всех задач
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>Коллекция всех задач, отсортированных по Id</returns>
        /// <remarks>Данные читаются из БД (ITaskStorage.GetAllAsync)</remarks>
        Task<IReadOnlyList<TTask>> GetAllTasksAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Выполнить задачу 
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<JobExecutionResult> ExecuteTaskAsync(TId taskId, CancellationToken ct = default);

        /// <summary>
        /// Инициализация оркестратора при старте приложения
        /// </summary>
        /// <remarks>
        /// Сбрасывает флаг IsRunning у всех задач (т.к. процессы уже не актуальны)
        /// и восстанавливает расписания.
        /// </remarks>
        Task InitializeAsync(CancellationToken ct = default);

    }
}