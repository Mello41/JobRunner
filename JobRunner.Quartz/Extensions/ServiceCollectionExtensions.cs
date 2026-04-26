using JobRunner.Core.Converters;
using JobRunner.Core.Interfaces.Scheduler;
using JobRunner.Quartz.Converters;
using JobRunner.Quartz.Scheduler;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace JobRunner.Quartz.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddQuartzScheduler(this IServiceCollection services)
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

            services.AddSingleton<ICronConverter, CronConverter>();
            services.AddSingleton<IJobScheduler, QuartzScheduler>();

            return services;
        }
    }
}
