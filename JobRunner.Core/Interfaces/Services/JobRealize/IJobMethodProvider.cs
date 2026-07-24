using JobRunner.Core.DTO.JobRealize;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JobRunner.Core.Interfaces.Services.JobRealize
{
    /// <summary>
    /// Провайдер методов реализации задач (низкий уровень)
    /// </summary>
    public interface IJobMethodProvider
    {
        /// <summary>
        /// Получить все доступные методы
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<List<JobRealizeMethodDto>> GetAvailableMethodsAsync(CancellationToken ct = default);

        /// <summary>
        /// Проверить, доступен ли метод
        /// </summary>
        /// <param name="methodType"></param>
        /// <returns></returns>
        bool IsMethodAvailable(string methodType);

        /// <summary>
        /// Получить метод по типу
        /// </summary>
        /// <param name="methodType"></param>
        /// <returns></returns>
        JobRealizeMethodDto? GetMethod(string methodType);
    }
}