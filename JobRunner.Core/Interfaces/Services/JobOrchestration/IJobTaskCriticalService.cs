using JobRunner.Core.DTO.Grouping;
using JobRunner.Core.Interfaces.Services.Detectors.JobTaskDetector;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JobRunner.Core.Interfaces.Services.JobOrchestration
{
    /// <summary>
    /// Сервис для работы с критическими ситуациями по задачам
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public interface IJobTaskCriticalService<TId> : IJobTaskDetector<TId>  
                                    where TId : IEquatable<TId>
    {
        /// <summary>
        /// Проверить и восстановить "зависшие" задачи (IsRunning = true, но процесс не жив)
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>Количество восстановленных задач</returns>
        Task<int> RecoverStuckTasksAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Выполнить "потерянные задачи"
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<int> ExecuteMissedSchedulesAsync(CancellationToken ct = default);

        /// <summary>
        /// Проверить группу на циклические зависимости и другие проблемы
        /// </summary>
        /// <param name="tagId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<GroupValidationResult<TId>> ValidateGroupAsync(TId tagId,
                                            CancellationToken ct = default);
    }
}