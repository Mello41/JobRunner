using JobRunner.Core.Interfaces.Core;
using JobRunner.Core.Interfaces.Entities;
using JobRunner.Core.Interfaces.Entities.EntityServices;
using JobRunner.Core.Interfaces.Events.DomainEvent;
using JobRunner.Core.Results;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JobRunner.Core.Interfaces
{
    /// <summary>
    /// В процессе выполнения задачи (управление жизненным циклом) нужно, 
    /// чтобы состояние выполнения задачи инкапсулировалось
    /// Инкапсулирует состояние выполнения задачи и предоставляет методы для управления жизненным циклом.
    /// </summary>
    /// Этот интерфейс гарантирует правильную последовательность операций:
    /// расшифровка → выполнение → шифрование.
    /// Реализация должна обеспечивать атомарность и защиту от утечек данных.
    /// </remarks>
    public interface IExecutionScope<TTask, TId>
                                where TTask : class, IJobTask<TId>
                                where TId : IEquatable<TId>
    {

        /// <summary>
        /// Задача, которая выполняется
        /// </summary>
        TTask Task { get; }

        /// <summary>
        /// Время старта выполнения (UTC)
        /// </summary>
        DateTime StartTime { get; }

        /// <summary>
        /// Расшифровывает чувствительные аргументы задачи
        /// </summary>
        Task DecryptArgumentsAsync(IEncryptionService encryption, CancellationToken cancellationToken = default);

        /// <summary>
        /// Публикует событие старта задачи
        /// </summary>
        Task PublishStartedEventAsync(IDomainEventDispatcher dispatcher, CancellationToken cancellationToken = default);

        /// <summary>
        /// Обновляет метаданные перед выполнением
        /// </summary>
        Task UpdateBeforeExecutionAsync(IJobTaskService<TTask, TId> storage, CancellationToken cancellationToken = default);

        /// <summary>
        /// Обновляет метаданные после выполнения
        /// </summary>
        Task UpdateAfterExecutionAsync(JobExecutionResult result, IJobTaskService<TTask, TId> storage, CancellationToken cancellationToken = default);

        /// <summary>
        /// Публикует событие завершения задачи
        /// </summary>
        Task PublishCompletedEventAsync(JobExecutionResult result, IDomainEventDispatcher dispatcher, CancellationToken cancellationToken = default);

        /// <summary>
        /// Обрабатывает отмену выполнения
        /// </summary>
        Task HandleCancellationAsync(OperationCanceledException ex, IJobTaskService<TTask, TId> storage, IDomainEventDispatcher dispatcher, CancellationToken cancellationToken = default);

        /// <summary>
        /// Обрабатывает ошибку выполнения
        /// </summary>
        Task HandleFailureAsync(Exception ex, IJobTaskService<TTask, TId> storage, IDomainEventDispatcher dispatcher, CancellationToken cancellationToken = default);

        /// <summary>
        /// Повторно шифрует аргументы (вызывается автоматически при Dispose)
        /// </summary>
        Task ReencryptArgumentsAsync(IEncryptionService encryption, CancellationToken cancellationToken = default);
    }
}
