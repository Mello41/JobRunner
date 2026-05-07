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
    /// <typeparam name="T"></typeparam>
    public interface ITagService<T> : ICrudService<T, Guid> where T : class, ITag
    {
        /// <summary>
        /// Установить цвет метки
        /// </summary>
        /// <param name="id">id метки</param>
        /// <param name="color">Цвет метки (#hex)</param>
        /// <returns></returns>
        Task<ITag> SetTagColor(Guid id, string color, CancellationToken cancellationToken = default);

        /// <summary>
        /// Посчитать количество задач, у которых есть данная метка
        /// </summary>
        /// <param name="id">id метки</param>
        /// <returns></returns>
        Task<ITag> CountJobsToTag(Guid id, CancellationToken cancellationToken = default);
    }
}
