using JobRunner.Core.DTO.TargetPlatform;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JobRunner.Core.Interfaces.Services.Detectors.TargetPlatform
{
    /// <summary>
    /// Реестр узлов (клиентов), которые могут выполнять задачи удалённо
    /// </summary>
    /// <typeparam name="TId">Тип идентификатора задачи</typeparam>
    public interface IJobNodeRegistry<TId> where TId : IEquatable<TId>
    {
        /// <summary>
        /// Зарегистрировать узел
        /// </summary>
        /// <param name="registration"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task RegisterNodeAsync(NodeRegistration registration, CancellationToken ct = default);

        /// <summary>
        /// Удалить узел из реестра
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task UnregisterNodeAsync(string nodeId, CancellationToken ct = default);

        /// <summary>
        /// Обновить heartbeat узла
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task HeartbeatAsync(string nodeId, CancellationToken ct = default);

        /// <summary>
        /// Получить все активные узлы
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<IReadOnlyList<NodeInfo>> GetActiveNodesAsync(CancellationToken ct = default);

        /// <summary>
        /// Получить узел по ID
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<NodeInfo?> GetNodeAsync(string nodeId, CancellationToken ct = default);

        /// <summary>
        /// Выбрать узел согласно целевым настройкам
        /// </summary>
        /// <param name="target"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<NodeInfo?> SelectNodeAsync(ExecutionTarget target, CancellationToken ct = default);

        /// <summary>
        /// Получить следующий узел в группе (round-robin)
        /// </summary>
        /// <param name="group"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<NodeInfo?> GetNextNodeInGroupAsync(string group, CancellationToken ct = default);
    }
}