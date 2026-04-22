using JobRunner.Core.Interfaces;
using JobRunner.Quartz.Scheduler;
using Microsoft.Extensions.DependencyInjection;

namespace JobRunner.Quartz.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Регистрация DI Quartz (метод расширения для DI регистрации)
        /// </summary>
        /// <param name="services">IServiceCollection - коллекция сервисов для цепочки вызовов</param>
        /// <returns></returns>
        public static IServiceCollection AddQuartzScheduler(this IServiceCollection services)
        {
            services.AddSingleton(typeof(IJobScheduler<>), typeof(QuartzScheduler<>));
            return services;
        }
    }
}
