using JobRunner.Core.DTO.JobRealize;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JobRunner.Core.Interfaces.Services.JobRealize
{
    /// <summary>
    /// Сервис для получения доступных способов реализации задач
    /// </summary>
    public interface IJobRealizeMethodsService
    {
        /// <summary>
        /// Получить все доступные способы реализации
        /// </summary>
        Task<List<JobRealizeMethodDto>> GetAvailableMethodsAsync(CancellationToken ct = default);

        /// <summary>
        /// Проверить, доступен ли конкретный способ
        /// </summary>
        Task<bool> IsMethodAvailableAsync(string methodType, CancellationToken ct = default);

        /// <summary>
        /// Получить метод по типу
        /// </summary>
        Task<JobRealizeMethodDto?> GetMethodAsync(string methodType, CancellationToken ct = default);

        /// <summary>
        /// Получить методы, сгруппированные по категориям
        /// </summary>
        Task<Dictionary<string, List<JobRealizeMethodDto>>> GetMethodsGroupedAsync(CancellationToken ct = default);
    }
}
