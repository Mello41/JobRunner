using JobRunner.Core.Entities;
using JobRunner.Core.Entities.ValueObjects;
using JobRunner.Core.Results;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace JobRunner.Core.DefaultImplementations
{
    /// <summary>
    /// Создаваемая задача (в рамках программы JobRunner) 
    /// (пример реализации)
    /// </summary>
    public class JobTask : IJobTask<Guid>
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
        public IRetrySettings RetrySettings { get; set; }

        public bool IsAsyncExecution { get; set; }
        public bool AllowConcurrentExecution { get; set; }

        /// <summary>
        /// Метки (задаются пользователем)
        /// </summary>
        public ImmutableHashSet<Guid> Tags { get; set; } = new Guid[0].ToImmutableHashSet<Guid>();

        public DomainValidationResult Validate()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(Name))
                errors.Add("Task name is required");

            if (string.IsNullOrWhiteSpace(ExecutionPath))
                errors.Add("Execution path is required");

            if (TimeoutSeconds < 0)
                errors.Add("Timeout cannot be negative");

            if (StartRun > EndRun && EndRun != default)
                errors.Add("StartRun cannot be after EndRun");

            if (ScheduleSettings != null && !ScheduleSettings.IsValid())
                errors.Add($"Schedule settings are invalid: {ScheduleSettings.GetDescription()}");

            return errors.Count == 0
                ? DomainValidationResult.Success()
                : DomainValidationResult.Fail(string.Join("; ", errors));
        }
    }
}
