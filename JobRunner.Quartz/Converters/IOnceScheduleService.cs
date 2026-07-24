using JobRunner.Core.DTO.ScheduleDTO;

namespace JobRunner.Quartz.Converters
{
    /// <summary>
    /// Сервис для работы с однократными расписаниями задач
    /// </summary>
    public interface IOnceScheduleService
    {
        /// <summary>
        /// Получить все расписания для задачи
        /// </summary>
        Task<List<OnceScheduleItem>> GetSchedulesForTaskAsync(long taskId, CancellationToken ct = default);

        /// <summary>
        /// Добавить однократное расписание
        /// </summary>
        Task AddScheduleAsync(long taskId, DateTime scheduledTime, CancellationToken ct = default);

        /// <summary>
        /// Добавить несколько однократных расписаний
        /// </summary>
        Task AddSchedulesAsync(long taskId, List<DateTime> scheduledTimes, CancellationToken ct = default);

        /// <summary>
        /// Отметить расписание как выполненное
        /// </summary>
        Task MarkAsCompletedAsync(long scheduleId, CancellationToken ct = default);

        /// <summary>
        /// Отметить расписание как выполненное по времени
        /// </summary>
        Task MarkAsCompletedAsync(long taskId, DateTime scheduledTime, CancellationToken ct = default);

        /// <summary>
        /// Удалить расписание
        /// </summary>
        Task DeleteScheduleAsync(long scheduleId, CancellationToken ct = default);

        /// <summary>
        /// Удалить все расписания для задачи
        /// </summary>
        Task DeleteSchedulesForTaskAsync(long taskId, CancellationToken ct = default);

        /// <summary>
        /// Получить следующее ожидающее расписание
        /// </summary>
        Task<DateTime?> GetNextPendingScheduleAsync(long taskId, CancellationToken ct = default);

        /// <summary>
        /// Проверить, есть ли ожидающие расписания
        /// </summary>
        Task<bool> HasPendingSchedulesAsync(long taskId, CancellationToken ct = default);

        /// <summary>
        /// Получить количество ожидающих расписаний
        /// </summary>
        Task<int> GetPendingCountAsync(long taskId, CancellationToken ct = default);

        /// <summary>
        /// Отключить расписание
        /// </summary>
        Task DisableScheduleAsync(long scheduleId, CancellationToken ct = default);

        /// <summary>
        /// Включить расписание
        /// </summary>
        Task EnableScheduleAsync(long scheduleId, CancellationToken ct = default);
    }
}
