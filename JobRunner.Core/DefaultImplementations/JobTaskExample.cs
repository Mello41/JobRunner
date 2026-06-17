using JobRunner.Core.DTO.TargetPlatform;
using JobRunner.Core.Entities.ValueObjects;
using JobRunner.Core.Interfaces.Entities;
using JobRunner.Core.Interfaces.Entities.JobTaskSettings;
using JobRunner.Core.Interfaces.Entities.JobTaskSettings.NotifySettings;
using JobRunner.Core.Interfaces.Entities.JobTaskSettings.ScheduleSettings;
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
    public class JobTaskExample : IJobTask<Guid>
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

        public INotifySettings<Guid> NotifySettings { get; set; }  = new NotifySettingsExample<Guid>();
        public IScheduleSettings ScheduleSettings { get; set; }  = new DailySchedule();
        public IScheduleArguments ScheduleArguments { get; set; } = new ScheduleArgumentsExample();
        public IJobTaskMetadata JobTaskMetadata { get; set; } = new JobTaskMetadataExample();
        public IRetrySettings RetrySettings { get; set; } = new RetrySettingsExample();

        public bool IsAsyncExecution { get; set; }
        public bool AllowConcurrentExecution { get; set; }

        /// <summary>
        /// Метки (задаются пользователем)
        /// </summary>
        public ImmutableHashSet<Guid> Tags { get; set; } = new Guid[0].ToImmutableHashSet<Guid>();
        public IGroupingSettings GroupingSettings { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ExecutionTarget ExecutionTarget { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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
