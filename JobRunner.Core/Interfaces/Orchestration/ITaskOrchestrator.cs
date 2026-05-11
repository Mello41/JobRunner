using JobRunner.Core.Entities;
using JobRunner.Core.Results;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JobRunner.Core.Interfaces.Orchestration
{
    /// <summary>
    /// Оркестратор задач (координация хранилища и планировщика)
    /// </summary>
    public interface ITaskOrchestrator<T> where T : IJobTask
    {
        /// <summary>
        /// Создание задачи и её регистрация в планировщике
        /// </summary>
        /// <param name="task">Задача для создания (Id может пусто быть)</param>
        /// <returns>T - созданная задача с присвоенным значением Id</returns>
        Task<T> CreateAndScheduleAsync(T task, CancellationToken cancellationToken = default);

        /// <summary>
        /// Обновление задачи и перерегистрация в планировщике
        /// </summary>
        /// <param name="task">Задача с обновлёнными данными</param>
        /// <returns>true — обновление успешно, 
        /// false — задача не найдена</returns>
        Task<bool> UpdateAndRescheduleAsync(T task, CancellationToken cancellationToken = default);

        /// <summary>
        /// Удаление задачи и её удаление из планировщика
        /// </summary>
        /// <param name="taskId">Уникальный идентификатор задачи</param>
        /// <returns> true — удаление успешно,
        /// false — задача не найдена </returns>
        Task<bool> DeleteAndUnscheduleAsync(Guid taskId, CancellationToken cancellationToken = default);

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
        Task<T?> GetTaskAsync(Guid taskId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получение всех задач
        /// </summary>
        /// <returns>Коллекция всех задач, отсортированных по Id</returns>
        /// <remarks>Данные читаются из БД (ITaskStorage.GetAllAsync)</remarks>
        Task<IReadOnlyList<T>> GetAllTasksAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Принудительный немедленный запуск задачи
        /// </summary>
        /// <param name="taskId">Уникальный идентификатор задачи</param>
        /// <returns>true — задача запущена, 
        /// false — задача не найдена</returns>
        /// <remarks>Вызывает IJobScheduler.RunNowAsync</remarks>
        Task<bool> RunNowAsync(Guid taskId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Приостановка выполнения задачи по расписанию
        /// </summary>
        /// <param name="taskId">Уникальный идентификатор задачи</param>
        /// <returns>true — задача приостановлена, 
        /// false — задача не найдена</returns>
        /// <remarks>Вызывает IJobScheduler.PauseAsync. 
        /// Может быть возобновлена через ResumeAsync</remarks>
        Task<bool> PauseAsync(Guid taskId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Возобновление выполнения задачи по расписанию
        /// </summary>
        /// <param name="taskId">Уникальный идентификатор задачи</param>
        /// <returns>true — задача возобновлена, 
        /// false — задача не найдена</returns>
        /// <remarks>Вызывает IJobScheduler.ResumeAsync</remarks>
        Task<bool> ResumeAsync(Guid taskId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Принудительная остановка выполняющейся задачи
        /// </summary>
        /// <param name="taskId">Уникальный идентификатор задачи</param>
        /// <returns>true — задача остановлена, 
        /// false — задача не найдена или не выполняется</returns>
        /// <remarks>Вызывает IJobScheduler.StopAsync. 
        /// Завершает процесс задачи.</remarks>
        Task<bool> StopAsync(Guid taskId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Выполнить задачу 
        /// </summary>
        Task<JobExecutionResult> ExecuteTaskAsync(Guid taskId, CancellationToken ct = default);

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
