using JobRunner.Core.Interfaces.Converters;
using JobRunner.Core.Interfaces.Entities;
using JobRunner.Core.Interfaces.Scheduler;
using JobRunner.Quartz.Converters;
using JobRunner.Quartz.Scheduler;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System.Security.Cryptography;

namespace JobRunner.Quartz.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddQuartzScheduler<TTask, TId>(this IServiceCollection services)
            where TTask : class, IJobTask<TId>
            where TId : IEquatable<TId>
        {
            services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();
                q.UseInMemoryStore();
            });

            services.AddQuartzHostedService(options =>
            {
                options.WaitForJobsToComplete = true;
            });

            services.AddSingleton<IScheduleConverter, CronConverter>();
            services.AddSingleton<IJobScheduler<TId>, QuartzScheduler<TTask, TId>>();

            return services;
        }
    }
}
