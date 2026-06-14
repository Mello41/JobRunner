using JobRunner.Core.DTO.Grouping;
using JobRunner.Core.Interfaces.Core;
using JobRunner.Core.Interfaces.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JobRunner.Core.Interfaces.Entities.EntityServices
{
    /// <summary>
    /// Набор методов для взаимодействия с метками
    /// </summary>
    /// <typeparam name="TTag">Тип метки, реализующий IJobTag&lt;TId&gt;</typeparam>
    /// <typeparam name="TId">Тип идентификатора</typeparam>
    public interface IJobTagService<TTag, TId> : ICrudService<TTag, TId>
                                where TTag : class, IJobTag<TId>
                                where TId : IEquatable<TId>
    {

        /// <summary>
        /// Установить цвет метки
        /// </summary>
        /// <param name="id">id метки</param>
        /// <param name="color">Цвет метки (#hex)</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TTag> SetColor(TId id, string color, CancellationToken cancellationToken = default);

        /// <summary>
        /// Посчитать количество задач, у которых есть данная метка
        /// </summary>
        /// <param name="id">id метки</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> CountJobsToTag(TId id, CancellationToken cancellationToken = default);

        #region Группировка задач по меткам
        /// <summary>
        /// Приостановить ВСЕ задачи с данной меткой
        /// </summary>
        Task<int> PauseAllByTagAsync(TId tagId, CancellationToken ct = default);

        /// <summary>
        /// Возобновить ВСЕ задачи с данной меткой
        /// </summary>
        Task<int> ResumeAllByTagAsync(TId tagId, CancellationToken ct = default);

        /// <summary>
        /// Запустить ВСЕ задачи с данной меткой сейчас
        /// </summary>
        Task<int> RunAllByTagAsync(TId tagId, CancellationToken ct = default);

        /// <summary>
        /// Получить сводку по группе (статистика)
        /// </summary>
        Task<TagGroupSummary> GetGroupSummaryAsync(TId tagId, CancellationToken ct = default);

        /// <summary>
        /// Применить настройки ко всем задачам группы
        /// </summary>
        Task<int> ApplySettingsToGroupAsync(TId tagId, GroupSettingsPatch patch, CancellationToken ct = default);
        #endregion
    }
}
