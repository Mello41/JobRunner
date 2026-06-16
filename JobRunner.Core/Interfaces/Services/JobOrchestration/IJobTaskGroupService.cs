using JobRunner.Core.DTO.Grouping;
using JobRunner.Core.Interfaces.Entities;
using JobRunner.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JobRunner.Core.Interfaces.Services.JobOrchestration
{
    /// <summary>
    /// Групповые операции над задачами по меткам
    /// </summary>
    /// <typeparam name="TTask"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public interface IJobTaskGroupService<TTask, TId>
                            where TTask : class, IJobTask<TId>
                            where TId : IEquatable<TId>
    {
        #region Получение данных
        /// <summary>
        /// Получить все задачи, привязанные к метке
        /// </summary>
        Task<IReadOnlyList<TTask>> GetTasksByTagAsync(TId tagId, CancellationToken ct = default);

        /// <summary>
        /// Получить сводку по группе (статистика)
        /// </summary>
        Task<TagGroupSummary> GetGroupSummaryAsync(TId tagId, CancellationToken ct = default);

        /// <summary>
        /// Получить сводки по всем группам
        /// </summary>
        Task<IReadOnlyList<TagGroupSummary>> GetAllGroupsSummaryAsync(CancellationToken ct = default);

        #endregion

        #region Управление связями

        /// <summary>
        /// Добавить задачу к метке
        /// </summary>
        Task<bool> AddTaskToTagAsync(TId tagId, TId taskId, CancellationToken ct = default);

        /// <summary>
        /// Удалить задачу из метки
        /// </summary>
        Task<bool> RemoveTaskFromTagAsync(TId tagId, TId taskId, CancellationToken ct = default);

        /// <summary>
        /// Добавить несколько задач к метке
        /// </summary>
        Task<int> AddTasksToTagAsync(TId tagId, IEnumerable<TId> taskIds, CancellationToken ct = default);

        /// <summary>
        /// Удалить несколько задач из метки
        /// </summary>
        Task<int> RemoveTasksFromTagAsync(TId tagId, IEnumerable<TId> taskIds, CancellationToken ct = default);

        /// <summary>
        /// Установить список меток для задачи (перезапись)
        /// </summary>
        Task<bool> SetTagsForTaskAsync(TId taskId, IEnumerable<TId> tagIds, CancellationToken ct = default);

        #endregion

        #region Групповые операции (по метке)

        /// <summary>
        /// Запустить ВСЕ задачи с данной меткой сейчас
        /// </summary>
        Task<int> RunAllByTagAsync(TId tagId, CancellationToken ct = default);

        /// <summary>
        /// Приостановить ВСЕ задачи с данной меткой
        /// </summary>
        Task<int> PauseAllByTagAsync(TId tagId, CancellationToken ct = default);

        /// <summary>
        /// Возобновить ВСЕ задачи с данной меткой
        /// </summary>
        Task<int> ResumeAllByTagAsync(TId tagId, CancellationToken ct = default);

        /// <summary>
        /// Остановить ВСЕ выполняющиеся задачи с данной меткой
        /// </summary>
        Task<int> StopAllByTagAsync(TId tagId, CancellationToken ct = default);

        /// <summary>
        /// Применить настройки ко всем задачам группы
        /// </summary>
        Task<int> ApplySettingsToGroupAsync(TId tagId, GroupSettingsPatch patch, CancellationToken ct = default);

        #endregion

        #region Последовательное выполнение группы (Pipeline)

        /// <summary>
        /// Выполнить группу задач последовательно (одна за другой)
        /// </summary>
        Task<GroupExecutionResult> ExecuteGroupSequentiallyAsync(
            TId tagId,
            CancellationToken ct = default);

        /// <summary>
        /// Выполнить группу задач с передачей данных между ними (конвейер)
        /// </summary>
        Task<GroupExecutionResult> ExecuteGroupPipelineAsync(
            TId tagId,
            CancellationToken ct = default);

        /// <summary>
        /// Выполнить группу задач с указанным режимом
        /// </summary>
        Task<GroupExecutionResult> ExecuteGroupAsync(
            TId tagId,
            GroupExecutionMode mode,
            CancellationToken ct = default);

        #endregion

        #region Проверки и валидация

        /// <summary>
        /// Проверить, есть ли у задачи указанная метка
        /// </summary>
        Task<bool> TaskHasTagAsync(TId taskId, TId tagId, CancellationToken ct = default);

        /// <summary>
        /// Проверить, можно ли выполнить группу (нет циклических зависимостей)
        /// </summary>
        Task<bool> ValidateGroupAsync(TId tagId, CancellationToken ct = default);

        /// <summary>
        /// Получить порядок выполнения задач в группе
        /// </summary>
        Task<IReadOnlyList<TTask>> GetGroupExecutionOrderAsync(TId tagId, CancellationToken ct = default);

        #endregion
    }
}