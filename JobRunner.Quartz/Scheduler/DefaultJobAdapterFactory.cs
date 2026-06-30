using JobRunner.Core.Interfaces.Entities;
using JobRunner.Core.Interfaces.Scheduler;
using JobRunner.Quartz.Adapters;

namespace JobRunner.Quartz.Scheduler
{
    /// <summary>
    /// Дефолтная фабрика, возвращающая JobAdapter{T task, TId}
    /// </summary>
    /// <typeparam name="TTask">Тип задачи</typeparam>
    /// <typeparam name="TId">Тип идентификатора</typeparam>
    public class DefaultJobAdapterFactory<TTask, TId> : IJobAdapterFactory<TTask, TId>
                                        where TTask : class, IJobTask<TId>
                                        where TId : IEquatable<TId>
    {
        public Type GetJobType() => typeof(JobAdapter<TTask, TId>);
    }
}
