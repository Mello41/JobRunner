using JobRunner.Core.Interfaces;
using JobRunner.Jobs.Encryption;
using JobRunner.Jobs.Scheduler;
using Microsoft.Extensions.DependencyInjection;

namespace JobRunner.Jobs.Extensions
{
    /// <summary>
    /// DI регистрация сервисов
    /// </summary>
    public static class ServiceCollectionExtensions 
    {
        /// <summary>
        /// Add все сервисы JobRunner в DI контейнер
        /// </summary>
        /// <param name="services"> Коллекция сервисов </param>
        /// <returns>возврат - коллекция сервисов для цепочки вызовов</returns>
        public static IServiceCollection AddJobs(this IServiceCollection services)
        {
            return services.AddSingleton<IStringEncryptor, AesStringEncryptor>(). // шифрование
                AddSingleton<IJobScheduler, QuartzJobScheduler>(); // планировщик задач
        }
    }
}
