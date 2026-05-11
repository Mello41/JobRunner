namespace JobRunner.Core.Entities.Enums
{
    /// <summary>
    /// Типы событий задач
    /// </summary>
    public enum EventType
    {
        /// <summary>
        /// Задача создана
        /// </summary>
        TaskCreated,

        /// <summary>
        /// Задача обновлена
        /// </summary>
        TaskUpdated,

        /// <summary>
        /// Задача удалена
        /// </summary>
        TaskDeleted,

        /// <summary>
        /// Задача запущена
        /// </summary>
        TaskStarted,

        /// <summary>
        /// Задача завершена (успешно или с ошибкой)
        /// </summary>
        TaskCompleted,

        /// <summary>
        /// Задача приостановлена
        /// </summary>
        TaskPaused,

        /// <summary>
        /// Задача возобновлена
        /// </summary>
        TaskResumed,

        /// <summary>
        /// Задача остановлена принудительно
        /// </summary>
        TaskStopped,

        /// <summary>
        /// Задача пропущена (например, из-за блокировки)
        /// </summary>
        TaskSkipped,

        /// <summary>
        /// Ошибка выполнения задачи
        /// </summary>
        TaskFailed,

        /// <summary>
        /// Задача истекла по таймауту
        /// </summary>
        TaskTimedOut
    }
}
