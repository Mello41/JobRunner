using JobRunner.Core.Interfaces.Entities;
using System;

namespace JobRunner.Core.Interfaces.Scheduler
{
    /// <summary>
    /// Фабрика для получения типа Job-адаптера
    /// </summary>
    /// <typeparam name="TTask">Тип задачи</typeparam>
    /// <typeparam name="TId">Тип идентификатора</typeparam>
    public interface IJobAdapterFactory<TTask, TId>
        where TTask : class, IJobTask<TId>
        where TId : IEquatable<TId>
    {
        /// <summary>
        /// Получить тип Job-адаптера для Quartz
        /// </summary>
        /// <returns>Тип, реализующий IJob</returns>
        Type GetJobType();
    }
}
