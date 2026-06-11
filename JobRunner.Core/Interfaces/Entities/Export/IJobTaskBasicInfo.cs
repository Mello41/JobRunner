using System;

namespace JobRunner.Core.Interfaces.Entities.Export
{
    /// <summary>
    /// Базовая информация о задаче (без чувствительных данных)
    /// </summary>
    public interface IJobTaskBasicInfo<TId> where TId : IEquatable<TId>
    {
        /// <summary>
        /// Идентификатор задачи
        /// </summary>
        TId Id { get; set; }

        /// <summary>
        /// Наименование задачи
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Описание задачи
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Путь к исполняемому файлу
        /// </summary>
        string ExecutionPath { get; set; }

        /// <summary>
        /// Таймаут выполнения в секундах
        /// </summary>
        int? TimeoutSeconds { get; set; }

        /// <summary>
        /// Разрешено ли параллельное выполнение
        /// </summary>
        bool AllowConcurrentExecution { get; set; }

        /// <summary>
        /// Асинхронное выполнение (не ждать завершения)
        /// </summary>
        bool IsAsyncExecution { get; set; }

        /// <summary>
        /// Дата начала действия задачи
        /// </summary>
        DateTime StartRun { get; set; }

        /// <summary>
        /// Дата окончания действия задачи
        /// </summary>
        DateTime EndRun { get; set; }

        /// <summary>
        /// Активна ли задача
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Статус выполнения (из метаданных)
        /// </summary>
        bool IsRunning { get; set; }

        /// <summary>
        /// Последний запуск
        /// </summary>
        DateTime? LastRun { get; set; }

        /// <summary>
        /// Следующий запуск
        /// </summary>
        DateTime? NextRun { get; set; }

        /// <summary>
        /// Общее количество запусков
        /// </summary>
        int TotalRunCount { get; set; }

        /// <summary>
        /// Количество успешных запусков
        /// </summary>
        int SuccessCount { get; set; }

        /// <summary>
        /// Количество неудачных запусков
        /// </summary>
        int FailureCount { get; set; }
    }
}
