using System.ComponentModel.DataAnnotations;

namespace JobRunner.Core.Entities.Enums
{
    /// <summary>
    /// Типы событий задач
    /// </summary>
    public enum EventType
    {
        [Display(Name = "Задача создана")]
        TaskCreated,

        [Display(Name = "Задача обновлена")]
        TaskUpdated,

        [Display(Name = "Задача удалена")]
        TaskDeleted,

        [Display(Name = "Задача запущена")]
        TaskStarted,

        [Display(Name = "Задача завершена (успешно или с ошибкой)")]
        TaskCompleted,

        [Display(Name = "Задача приостановлена")]
        TaskPaused,

        [Display(Name = "Задача возобновлена")]
        TaskResumed,

        [Display(Name = "Задача остановлена принудительно")]
        TaskStopped,

        [Display(Name = "Задача пропущена (например, из-за блокировки)")]
        TaskSkipped,

        [Display(Name = "Ошибка выполнения задачи")]
        TaskFailed,

        [Display(Name = "Задача истекла по таймауту")]
        TaskTimedOut
    }
}