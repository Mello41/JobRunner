using JobRunner.Core.DTO.Recovery;
using JobRunner.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JobRunner.Core.Interfaces.Services.Detectors.JobTaskDetector
{
    /// <summary>
    /// обнаружение проблем (read-only)
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public interface IJobTaskDetector<TId> where TId : IEquatable<TId>
    {
        /// <summary>
        /// Обнаружить "зависшие" задачи (IsRunning = true, но процесс не жив)
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<List<StuckTaskInfo<TId>>> DetectStuckTasksAsync(CancellationToken ct = default);

        /// <summary>
        /// Обнаружить пропущенные запуски (сервер был выключен)
        /// </summary>
        /// <param name="since"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<List<MissedScheduleInfo<TId>>> DetectMissedSchedulesAsync(
            DateTime? since = null,
            CancellationToken ct = default);

        /// <summary>
        /// Обнаружить задачи, подозреваемые в бесконечном выполнении
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<List<InfiniteLoopSuspicion<TId>>> DetectInfiniteLoopsAsync(
            CancellationToken ct = default);

        /// <summary>
        /// Обнаружить задачи с истощением попыток (Retry Exhaustion)
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<List<RetryExhaustionInfo<TId>>> DetectRetryExhaustionAsync(
            CancellationToken ct = default);

        /// <summary>
        /// Получить общее количество проблемных задач
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<CriticalIssuesSummary> GetCriticalIssuesSummaryAsync(
            CancellationToken ct = default);

    }
}