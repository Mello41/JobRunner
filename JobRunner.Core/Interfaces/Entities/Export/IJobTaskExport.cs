using JobRunner.Core.Interfaces.Entities.JobTaskSettings;
using JobRunner.Core.Interfaces.Entities.JobTaskSettings.NotifySettings;
using JobRunner.Core.Interfaces.Entities.JobTaskSettings.ScheduleSettings;
using System;
using System.Collections.Generic;

namespace JobRunner.Core.Interfaces.Entities.Export
{
    /// <summary>
    /// Интерфейс для экспорта/импорта задачи
    /// </summary>
    /// <typeparam name="TId">Тип идентификатора задачи</typeparam>
    public interface IJobTaskExport<TId> where TId : IEquatable<TId>
    {
        /// <summary>
        /// Версия формата экспорта
        /// </summary>
        string Version { get; set; }

        /// <summary>
        /// Дата экспорта
        /// </summary>
        DateTime ExportedAt { get; set; }

        /// <summary>
        /// Кто выполнил экспорт
        /// </summary>
        string ExportedBy { get; set; }

        /// <summary>
        /// Описание экспорта (опционально)
        /// </summary>
        string? Description { get; set; }

        /// <summary>
        /// Основная информация о задаче
        /// </summary>
        IJobTaskBasicInfo<TId> BasicInfo { get; set; }

        /// <summary>
        /// Настройки расписания
        /// </summary>
        IScheduleSettings ScheduleSettings { get; set; }

        /// <summary>
        /// Настройки уведомлений
        /// </summary>
        INotifySettings<TId> NotifySettings { get; set; }

        /// <summary>
        /// Аргументы командной строки
        /// </summary>
        IScheduleArguments ScheduleArguments { get; set; }

        /// <summary>
        /// Настройки повторных попыток
        /// </summary>
        IRetrySettings RetrySettings { get; set; }

        /// <summary>
        /// Список тегов (ID)
        /// </summary>
        List<TId> Tags { get; set; }

        /// <summary>
        /// Дополнительные метаданные (опционально)
        /// </summary>
        Dictionary<string, string>? Metadata { get; set; }
    }
}
