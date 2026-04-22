using JobRunner.Core.Entities;
using System;
using System.Threading.Tasks;

namespace JobRunner.Core.Interfaces
{
    /// <summary>
    /// Сборник методов планировщика заданий (операции с памятью и выполнением)
    /// </summary>
    /// <typeparam name="T">Тип задачи, реализующий IJobTask</typeparam>
    public interface IJobScheduler<T> where T : IJobTask
    {
        #region Планировщик (жизненный цикл)
        /// <summary>
        /// Запуск программы
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Инициализирует внутренний планировщик (Quartz), загружает сохранённые задачи
        /// и начинает отслеживание расписаний. Должен быть вызван перед любыми другими операциями.
        /// </remarks>
        Task StartProgramAsync();

        /// <summary>
        /// Остановка программы
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Останавливает все активные задачи, завершает работу внутреннего планировщика.
        /// Состояние задач сохраняется для последующего восстановления.
        /// </remarks>
        Task StopProgramAsync();
        #endregion

        #region Задача - Управление выполнением
        /// <summary>
        /// Принудительный немедленный запуск задачи
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        Task<bool> RunNowAsync(Guid taskId);

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
        Task<bool> RestartAsync(Guid taskId);

        /// <summary>
        /// Приостановка выполнения задачи по расписанию
        /// </summary>
        /// <param name="taskId">Уникальный идентификатор задачи</param>
        /// <returns>true — задача приостановлена, false — задача не найдена</returns>
        /// <remarks>
        /// Задача остаётся в системе, но не будет запускаться по расписанию.
        /// Может быть возобновлена методом ResumeAsync.
        /// </remarks>
        Task<bool> PauseAsync(Guid taskId);

        /// <summary>
        /// Остановка выполняющейся задачи (принудительное завершение)
        /// </summary>
        /// <param name="taskId">Уникальный идентификатор задачи</param>
        /// <returns>true — задача остановлена, false — задача не найдена или не выполняется</returns>
        /// <remarks>
        /// Принудительно завершает процесс, связанный с задачей.
        /// Состояние задачи обновляется, ошибка фиксируется в LastError.
        /// </remarks>
        Task<bool> StopAsync(Guid taskId);

        /// <summary>
        /// Возобновление выполнения задачи по расписанию
        /// </summary>
        /// <param name="taskId">Уникальный идентификатор задачи</param>
        /// <returns>true — задача возобновлена, false — задача не найдена</returns>
        /// <remarks>
        /// Восстанавливает выполнение ранее приостановленной задачи.
        /// Расписание сохраняется и начинает отсчитываться заново.
        /// </remarks>
        Task<bool> ResumeAsync(Guid taskId);
        #endregion
    }
}
