using JobRunner.Core.Interfaces;
using JobRunner.WindowsService.Scheduler;
using Microsoft.Extensions.DependencyInjection;

namespace JobRunner.WindowsService.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWindowsTaskScheduler(this IServiceCollection services)
        {
            services.AddSingleton<IJobScheduler, WindowsTaskScheduler>();
            return services;
        }
    }
}
