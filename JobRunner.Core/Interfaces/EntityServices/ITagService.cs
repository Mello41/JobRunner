using JobRunner.Core.Entities;
using JobRunner.Core.Interfaces.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JobRunner.Core.Interfaces.EntityServices
{
    /// <summary>
    /// Набор методов для взаимодействия с метками
    /// </summary>
    /// <typeparam name="TTag">Тип метки, реализующий IJobTag&lt;TId&gt;</typeparam>
    /// <typeparam name="TId">Тип идентификатора</typeparam>
    public interface ITagService<TTag, TId> : ICrudService<TTag, TId>
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
        Task<TTag> SetTagColor(TId id, string color, CancellationToken cancellationToken = default);

        /// <summary>
        /// Посчитать количество задач, у которых есть данная метка
        /// </summary>
        /// <param name="id">id метки</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> CountJobsToTag(TId id, CancellationToken cancellationToken = default);
    }
}
