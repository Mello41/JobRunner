using JobRunner.Core.Interfaces.Converters;
using JobRunner.Core.Interfaces.Entities;
using JobRunner.Core.Interfaces.Scheduler;
using JobRunner.Quartz.Converters;
using JobRunner.Quartz.Scheduler;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace JobRunner.Quartz.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddQuartzScheduler<TTask, TId>(
            this IServiceCollection services,
            Action<IServiceCollectionQuartzConfigurator>? configureQuartz = null,  
            bool enableLoggingListener = true)
            where TTask : class, IJobTask<TId>
            where TId : IEquatable<TId>
        {
            services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();
                q.UseInMemoryStore();

                if (enableLoggingListener)
                {
                    q.AddJobListener<JobLoggingListener>();
                }
                configureQuartz?.Invoke(q);
            });

            services.AddQuartzHostedService(options =>
            {
                options.WaitForJobsToComplete = true;
            });

            services.AddSingleton<IScheduleConverter, CronConverter>();
            services.AddSingleton<IJobTaskScheduler<TId>, QuartzScheduler<TTask, TId>>();

            if (enableLoggingListener)
            {
                services.AddSingleton<JobLoggingListener>();
                services.AddSingleton<IJobExecutionListener, JobLoggingListener>();
            }

            return services;
        }
    }
}
