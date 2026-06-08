using JobRunner.Core.Entities.ValueObjects;
using JobRunner.Core.Results;
using System;
using System.Collections.Immutable;

namespace JobRunner.Core.Entities
{
    /// <summary>
    /// Интерфейс задачи (создаваемая задача в рамках программы JobRunner)
    /// </summary>
    public interface IJobTask<TId> where TId : IEquatable<TId>
    {
        /// <summary>
        /// ID задачи
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
        /// Путь к исполняемому файлу задачи
        /// </summary>
        string ExecutionPath { get; set; }

        /// <summary>
        /// Дата начала выполнения задачи
        /// </summary>
        DateTime StartRun { get; set; }

        /// <summary>
        /// Дата окончания действия задачи
        /// </summary>
        DateTime EndRun { get; set; }

        /// <summary>
        /// Задача активна? (вкл/выкл)
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Таймаут выполнения в секундах (null = без таймаута)
        /// Если задача выполняется дольше указанного времени, она принудительно завершается
        /// </summary>
        int? TimeoutSeconds { get; set; }

        /// <summary>
        /// Настройки уведомлений задачи
        /// </summary>
        INotifySettings NotifySettings { get; set; }

        /// <summary>
        /// Параллельное выполнение?
        /// IsAsyncExecution = true -->	Не ждать завершения процесса
        /// IsAsyncExecution = false --> Ждать завершения процесса
        /// </summary>
        bool IsAsyncExecution { get; set; }

        /// <summary>
        /// Разрешить параллельное выполнение (несколько экземпляров одновременно)
        /// AllowConcurrentExecution = true -->	Разрешить несколько экземпляров задачи одновременно
        /// AllowConcurrentExecution = false --> Запретить несколько экземпляров
        /// </summary>
        bool AllowConcurrentExecution { get; set; }

        /// <summary>
        /// Настройки периодичности
        /// </summary>
        IScheduleSettings ScheduleSettings { get; set; }

        /// <summary>
        /// Аргументы задачи (с возможностью шифрования)
        /// </summary>
        IScheduleArguments ScheduleArguments { get; set; }

        /// <summary>
        /// Метаданные задачи
        /// </summary>
        IJobTaskMetadata JobTaskMetadata { get; set; }

        /// <summary>
        /// 
        /// </summary>
        IRetrySettings RetrySettings { get; set; }

        /// <summary>
        /// Метки (задаются пользователем)
        /// </summary>
        ImmutableHashSet<TId> Tags { get; set; }

        /// <summary>
        /// Проверяет бизнес-инварианты модели (не зависит от пользовательского ввода)
        /// </summary>
        /// <returns>Коллекция ошибок или null, если всё корректно</returns>
        DomainValidationResult Validate();
    }
}
