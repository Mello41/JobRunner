using System;
using System.Collections.Concurrent;

namespace JobRunner.Core.Entities
{
    /// <summary>
    /// Интерфейс задачи (создаваемая задача в рамках программы JobRunner)
    /// </summary>
    public interface IJobTask
    {
        /// <summary>
        /// ID задачи
        /// </summary>
        Guid Id { get; set; }

        /// <summary>
        /// Process ID задачи (существует только тогда, когда задача запущена)
        /// </summary>
        long? PID { get; set; }

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
        /// Задача запущена сейчас?
        /// </summary>
        bool IsRunning { get; set; }

        /// <summary>
        /// Задача завершена?
        /// </summary>
        bool IsCompleted { get; set; }

        /// <summary>
        /// Дата последнего запуска
        /// </summary>
        DateTime? LastRun { get; set; }

        /// <summary>
        /// Дата следующего запуска
        /// </summary>
        DateTime? NextRun { get; set; }

        /// <summary>
        /// Настройки уведомлений задачи
        /// </summary>
        INotifySettings NotifySettings { get; set; }

        /// <summary>
        /// Параллельное выполнение?
        /// </summary>
        bool IsAsyncExecution { get; set; }

        /// <summary>
        /// Настройки периодичности
        /// </summary>
        IScheduleSettings ScheduleSettings { get; set; }

        /// <summary>
        /// Последняя ошибка
        /// </summary>
        string LastError { get; set; }

        /// <summary>
        /// Аргументы задачи (с возможностью шифрования)
        /// </summary>
        IScheduleArguments ScheduleArguments { get; set; }

        /// <summary>
        /// Метки (задаются пользователем)
        /// </summary>
        ConcurrentDictionary<Guid, ITag> Tags { get; set; }
    }
}
