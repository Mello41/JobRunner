using JobRunner.Core.Entities;
using JobRunner.Core.Entities.ValueObjects;
using JobRunner.Core.Results;
using System;
using System.Collections.Generic;

namespace JobRunner.Core.DefaultImplementations
{
    /// <summary>
    /// Создаваемая задача (в рамках программы JobRunner) 
    /// (пример реализации)
    /// </summary>
    public class JobTask : IJobTask
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ExecutionPath { get; set; } = string.Empty;

        #region Состояние выполнения задачи
        public DateTime StartRun { get; set; }
        public DateTime EndRun { get; set; }
        public bool IsEnabled { get; set; } = true;

        public int? TimeoutSeconds { get; set; }
        #endregion

        public INotifySettings NotifySettings { get; set; } = new NotifySettings();
        public IScheduleSettings ScheduleSettings { get; set; } // = new CronSchedule();
        public IScheduleArguments ScheduleArguments { get; set; } = new ScheduleArguments();
        public IJobTaskMetadata JobTaskMetadata { get; set; } = new JobTaskMetadata();

        public bool IsAsyncExecution { get; set; }
        public bool AllowConcurrentExecution { get; set; }

        /// <summary>
        /// Метки (задаются пользователем)
        /// </summary>
        public IReadOnlyList<Guid> Tags { get; set; } = new List<Guid>();

        public DomainValidationResult Validate()
        {
            throw new NotImplementedException();
        }
    }
}
