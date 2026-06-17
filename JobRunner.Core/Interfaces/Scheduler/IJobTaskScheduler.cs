using JobRunner.Core.Interfaces.Entities;
using JobRunner.Core.Interfaces.Entities.JobTaskSettings.ScheduleSettings;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JobRunner.Core.Interfaces.Scheduler
{
    /// <summary>
    /// Сборник методов планировщика заданий (операции с памятью и выполнением)
    /// </summary>
    /// <typeparam name="TId">Тип идентификатора задачи</typeparam>
    public interface IJobTaskScheduler<TId> where TId : IEquatable<TId>
    {
        #region Планировщик (жизненный цикл)
        /// <summary>
        /// Запуск программы
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <remarks>
        /// Инициализирует внутренний планировщик (Quartz), загружает сохранённые задачи
        /// и начинает отслеживание расписаний. Должен быть вызван перед любыми другими операциями.
        /// </remarks>
        Task StartProgramAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Остановка программы
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <remarks>
        /// Останавливает все активные задачи, завершает работу внутреннего планировщика.
        /// Состояние задач сохраняется для последующего восстановления.
        /// </remarks>
        Task StopProgramAsync(CancellationToken cancellationToken = default);
        #endregion

        #region Задача - Управление выполнением
        /// <summary>
        /// Принудительный немедленный запуск задачи
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        Task<bool> RunNowAsync(TId taskId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Перезапуск задачи (остановить и запустить заново)
        /// </summary>
        /// <param name="taskId">Уникальный идентификатор задачи</param>
        /// <returns>true — задача успешно перезапущена, false — ошибка</returns>
        /// <remarks>
        /// 1. Останавливает текущий процесс (если запущен)
        /// 2. Сбрасывает состояние задачи (LastError, IsRunning)
        /// 3. Запускает задачу заново
        /// </remarks>
        Task<bool> RestartAsync(TId taskId, int delay = 100, CancellationToken cancellationToken = default);

        /// <summary>
        /// Приостановка выполнения задачи по расписанию
        /// </summary>
        /// <param name="taskId">Уникальный идентификатор задачи</param>
        /// <returns>true — задача приостановлена, false — задача не найдена</returns>
        /// <remarks>
        /// Задача остаётся в системе, но не будет запускаться по расписанию.
        /// Может быть возобновлена методом ResumeAsync.
        /// </remarks>
        Task<bool> PauseAsync(TId taskId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Остановка выполняющейся задачи (принудительное завершение)
        /// </summary>
        /// <param name="taskId">Уникальный идентификатор задачи</param>
        /// <returns>true — задача остановлена, false — задача не найдена или не выполняется</returns>
        /// <remarks>
        /// Принудительно завершает процесс, связанный с задачей.
        /// Состояние задачи обновляется, ошибка фиксируется в LastError.
        /// </remarks>
        Task<bool> StopAsync(TId taskId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Возобновление выполнения задачи по расписанию
        /// </summary>
        /// <param name="taskId">Уникальный идентификатор задачи</param>
        /// <returns>true — задача возобновлена, false — задача не найдена</returns>
        /// <remarks>
        /// Восстанавливает выполнение ранее приостановленной задачи.
        /// Расписание сохраняется и начинает отсчитываться заново.
        /// </remarks>
        Task<bool> ResumeAsync(TId taskId, CancellationToken cancellationToken = default);
        #endregion

        #region Управление расписанием
        /// <summary>
        /// Зарегистрировать задачу в планировщике
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="schedule"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task ScheduleAsync(TId taskId, IScheduleSettings schedule, CancellationToken cancellationToken = default);

        /// <summary>
        /// Удалить задачу из планировщика
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task UnscheduleAsync(TId taskId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Обновить расписание задачи
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="schedule"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task RescheduleAsync(TId taskId, IScheduleSettings schedule, CancellationToken cancellationToken = default);

        /// <summary>
        /// Перерегистрировать все задачи из БД в Quartz (восстановление после перезапуска)
        /// </summary>
        /// <param name="tasks"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task RestoreSchedulesAsync(IEnumerable<IJobTask<TId>> tasks, CancellationToken ct = default);
        #endregion
    }
}
