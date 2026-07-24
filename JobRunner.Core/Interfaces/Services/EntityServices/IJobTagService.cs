using JobRunner.Core.DTO.Grouping;
using JobRunner.Core.Interfaces.Core;
using JobRunner.Core.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JobRunner.Core.Interfaces.Services.EntityServices
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

        /// <summary>
        /// Получить метки по задаче
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<IEnumerable<TTag>> GetTagsForTaskAsync(TId taskId, CancellationToken ct = default);
    }
}