using JobRunner.Core.DefaultImplementations;
using JobRunner.Core.Entities.ValueObjects;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

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
        /// Аргументы задачи (с возможностью шифрования)
        /// </summary>
        IScheduleArguments ScheduleArguments { get; set; }

        IJobTaskMetadata JobTaskMetadata { get; set; }

        /// <summary>
        /// Метки (задаются пользователем)
        /// </summary>
        IReadOnlyList<Guid> Tags { get; set; }
    }
}
