using JobRunner.Core.Models.Enums;

namespace JobRunner.Core.Interfaces.Entities.JobTaskSettings
{
    /// <summary>
    /// Настройки группового выполнения задач
    /// </summary>
    /// <remarks>
    /// Реализуется IJobTag для поддержки групповых операций
    /// </remarks>
    public interface IGroupingSettings
    {
        /// <summary>
        /// Разрешены ли групповые операции 
        /// над задачами этой метки
        /// </summary>
        bool AllowGroupOperations { get; set; }

        /// <summary>
        /// Режим выполнения задач в группе
        /// </summary>
        GroupExecutionMode ExecutionMode { get; set; }

        /// <summary>
        /// Задержка между задачами в группе (секунды)
        /// </summary>
        /// <remarks>
        /// Используется только при ExecutionMode = Sequential
        /// </remarks>
        int? DelayBetweenTasksSeconds { get; set; }

        /// <summary>
        /// Остановить выполнение группы при первой ошибке
        /// </summary>
        bool StopGroupOnFirstFailure { get; set; }

        /// <summary>
        /// Максимальное количество параллельных задач в группе
        /// </summary>
        /// <remarks>
        /// Используется только при ExecutionMode = Parallel
        /// null = без ограничений
        /// </remarks>
        int? MaxConcurrentTasksInGroup { get; set; }

        /// <summary>
        /// Расписание для ВСЕЙ группы
        /// Если задано, переопределяет индивидуальные расписания задач
        /// </summary>
        IScheduleSettings? GroupSchedule { get; set; }

        /// <summary>
        /// Требовать успешное выполнение предыдущей задачи
        /// </summary>
        bool RequirePreviousTaskSuccess { get; set; }

        /// <summary>
        /// Таймаут выполнения всей группы (секунды)
        /// </summary>
        int? GroupTimeoutSeconds { get; set; }

        /// <summary>
        /// Приоритет группы (выше приоритет — 
        /// раньше выполняется)
        /// </summary>
        int GroupPriority { get; set; }
    }
}