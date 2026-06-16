using JobRunner.Core.Interfaces.Core;
using JobRunner.Core.Interfaces.Entities;
using JobRunner.Core.Interfaces.Events.DomainEvent;
using JobRunner.Core.Interfaces.Services.EntityServices;
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
        /// <param name="encryption"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DecryptArgumentsAsync(IEncryptionService encryption, CancellationToken cancellationToken = default);

        /// <summary>
        /// Публикует событие старта задачи
        /// </summary>
        /// <param name="dispatcher"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task PublishStartedEventAsync(IDomainEventDispatcher dispatcher, CancellationToken cancellationToken = default);

        /// <summary>
        /// Обновляет метаданные перед выполнением
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task UpdateBeforeExecutionAsync(IJobTaskService<TTask, TId> storage, CancellationToken cancellationToken = default);

        /// <summary>
        /// Обновляет метаданные после выполнения
        /// </summary>
        /// <param name="result"></param>
        /// <param name="storage"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task UpdateAfterExecutionAsync(JobExecutionResult result, IJobTaskService<TTask, TId> storage, CancellationToken cancellationToken = default);

        /// <summary>
        /// Публикует событие завершения задачи
        /// </summary>
        /// <param name="result"></param>
        /// <param name="dispatcher"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task PublishCompletedEventAsync(JobExecutionResult result, IDomainEventDispatcher dispatcher, CancellationToken cancellationToken = default);

        /// <summary>
        /// Обрабатывает отмену выполнения
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="storage"></param>
        /// <param name="dispatcher"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task HandleCancellationAsync(OperationCanceledException ex, IJobTaskService<TTask, TId> storage, IDomainEventDispatcher dispatcher, CancellationToken cancellationToken = default);

        /// <summary>
        /// Обрабатывает ошибку выполнения
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="storage"></param>
        /// <param name="dispatcher"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task HandleFailureAsync(Exception ex, IJobTaskService<TTask, TId> storage, IDomainEventDispatcher dispatcher, CancellationToken cancellationToken = default);

        /// <summary>
        /// Повторно шифрует аргументы 
        /// (вызывается автоматически при Dispose)
        /// </summary>
        /// <param name="encryption"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task ReencryptArgumentsAsync(IEncryptionService encryption, CancellationToken cancellationToken = default);
    }
}
