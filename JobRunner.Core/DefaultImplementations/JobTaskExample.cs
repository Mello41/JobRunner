using JobRunner.Core.DTO.Results;
using JobRunner.Core.DTO.TargetPlatform;
using JobRunner.Core.Interfaces.Entities;
using JobRunner.Core.Interfaces.Entities.JobTaskSettings;
using JobRunner.Core.Interfaces.Entities.JobTaskSettings.NotifySettings;
using JobRunner.Core.Interfaces.Entities.JobTaskSettings.ScheduleSettings;
using JobRunner.Core.Models.ValueSchedule;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace JobRunner.Core.DefaultImplementations
{
    /// <summary>
    /// Полноценная реализация задачи для тестирования и примеров
    /// </summary>
    public class JobTaskExample : IJobTask<Guid>
    {
        #region Базовые свойства

        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartRun { get; set; } = DateTime.Now;
        public DateTime EndRun { get; set; } = DateTime.Now.AddMonths(1);
        public bool IsEnabled { get; set; } = true;
        public int? TimeoutSeconds { get; set; }

        #endregion

        #region Свойства выполнения

        public bool UseCustomExecution { get; set; }
        public string ExecutionPath { get; set; } = string.Empty;
        public string MethodName { get; set; } = string.Empty;

        #endregion

        #region Разрешения

        public bool CanGrouping { get; set; } = true;

        #endregion

        #region Параллельное выполнение

        public bool IsAsyncExecution { get; set; }
        public bool AllowConcurrentExecution { get; set; }

        #endregion

        #region Настройки выполнения задачи

        public Guid NotifySettingsId { get; set; }
        public INotifySettings<Guid> NotifySettings { get; set; } = new NotifySettingsExample<Guid>();
        public IScheduleSettings ScheduleSettings { get; set; } = new DailySchedule();
        public IScheduleArguments ScheduleArguments { get; set; } = new ScheduleArgumentsExample();
        public IJobTaskMetadata JobTaskMetadata { get; set; } = new JobTaskMetadataExample();
        public IRetrySettings RetrySettings { get; set; } = new RetrySettingsExample();
        public ExecutionTarget ExecutionTarget { get; set; } = ExecutionTarget.Server();

        #endregion

        #region Метки и группировка

        public ImmutableHashSet<Guid> Tags { get; set; } = ImmutableHashSet<Guid>.Empty;

        /// <summary>
        /// Настройки группировки (для меток)
        /// </summary>
        public IGroupingSettings GroupingSettings { get; set; } // = new GroupingSettingsExample();

        #endregion

        #region Конструкторы

        public JobTaskExample() { }

        /// <summary>
        /// Создать задачу с минимальными параметрами
        /// </summary>
        public JobTaskExample(string name, string executionPath)
        {
            Name = name;
            ExecutionPath = executionPath;
        }

        /// <summary>
        /// Создать задачу с полными параметрами
        /// </summary>
        public JobTaskExample(
            string name,
            string executionPath,
            IScheduleSettings scheduleSettings,
            bool isEnabled = true,
            int? timeoutSeconds = null)
        {
            Name = name;
            ExecutionPath = executionPath;
            ScheduleSettings = scheduleSettings ?? new DailySchedule();
            IsEnabled = isEnabled;
            TimeoutSeconds = timeoutSeconds;
        }

        #endregion

        #region Fluent API для удобного создания

        public JobTaskExample WithName(string name)
        {
            Name = name;
            return this;
        }

        public JobTaskExample WithDescription(string description)
        {
            Description = description;
            return this;
        }

        public JobTaskExample WithExecutionPath(string path)
        {
            ExecutionPath = path;
            return this;
        }

        public JobTaskExample WithSchedule(IScheduleSettings schedule)
        {
            ScheduleSettings = schedule;
            return this;
        }

        public JobTaskExample WithTimeout(int seconds)
        {
            TimeoutSeconds = seconds;
            return this;
        }

        public JobTaskExample WithCustomExecution(string methodName)
        {
            UseCustomExecution = true;
            MethodName = methodName;
            return this;
        }

        public JobTaskExample Enabled(bool enabled = true)
        {
            IsEnabled = enabled;
            return this;
        }

        public JobTaskExample WithAsyncExecution(bool async = true)
        {
            IsAsyncExecution = async;
            return this;
        }

        public JobTaskExample WithConcurrentExecution(bool concurrent = true)
        {
            AllowConcurrentExecution = concurrent;
            return this;
        }

        public JobTaskExample WithTags(params Guid[] tags)
        {
            Tags = tags.ToImmutableHashSet();
            return this;
        }

        public JobTaskExample WithRetrySettings(IRetrySettings retrySettings)
        {
            RetrySettings = retrySettings;
            return this;
        }

        public JobTaskExample WithArguments(IScheduleArguments arguments)
        {
            ScheduleArguments = arguments;
            return this;
        }

        #endregion

        #region Валидация

        public DomainValidationResult Validate()
        {
            var errors = new List<string>();

            // Основные проверки
            if (string.IsNullOrWhiteSpace(Name))
                errors.Add("Task name is required");

            if (Name.Length > 255)
                errors.Add("Task name cannot exceed 255 characters");

            // Проверка пути выполнения
            if (!UseCustomExecution && string.IsNullOrWhiteSpace(ExecutionPath))
                errors.Add("Execution path is required for file execution");

            if (UseCustomExecution && string.IsNullOrWhiteSpace(MethodName))
                errors.Add("Method name is required for custom execution");

            if (TimeoutSeconds < 0)
                errors.Add("Timeout cannot be negative");

            if (TimeoutSeconds > 86400)
                errors.Add("Timeout cannot exceed 24 hours (86400 seconds)");

            if (StartRun > EndRun && EndRun != default)
                errors.Add("StartRun cannot be after EndRun");

            // Проверка расписания
            if (ScheduleSettings != null && !ScheduleSettings.IsValid())
                errors.Add($"Schedule settings are invalid: {ScheduleSettings.GetDescription()}");

            // Проверка настроек уведомлений (если включены)
            if (NotifySettings != null && NotifySettings.EnableNotifications)
            {
                var hasActiveRecipients = false;
                foreach (var statusSettings in NotifySettings.StatusSettings.Values)
                {
                    if (statusSettings.IsEnabled && statusSettings.Recipients.Count > 0)
                    {
                        hasActiveRecipients = true;
                        break;
                    }
                }

                if (!hasActiveRecipients)
                    errors.Add("Notifications are enabled but no recipients configured");
            }

            // Проверка аргументов (если есть)
            if (ScheduleArguments?.Items != null)
            {
                foreach (var arg in ScheduleArguments.Items)
                {
                    if (string.IsNullOrWhiteSpace(arg.Key))
                        errors.Add("Argument key cannot be empty");

                    if (arg.IsEncryptedArgument && arg.Value == null)
                        errors.Add($"Argument '{arg.Key}' is marked as encrypted but has no value");
                }
            }

            return errors.Count == 0
                ? DomainValidationResult.Success()
                : DomainValidationResult.Fail(string.Join("; ", errors));
        }

        #endregion

        #region Вспомогательные методы

        /// <summary>
        /// Создать копию задачи (без ID)
        /// </summary>
        public JobTaskExample Clone()
        {
            return new JobTaskExample
            {
                Name = $"{Name} (Copy)",
                Description = Description,
                ExecutionPath = ExecutionPath,
                UseCustomExecution = UseCustomExecution,
                MethodName = MethodName,
                StartRun = StartRun,
                EndRun = EndRun,
                IsEnabled = IsEnabled,
                TimeoutSeconds = TimeoutSeconds,
                IsAsyncExecution = IsAsyncExecution,
                AllowConcurrentExecution = AllowConcurrentExecution,
                CanGrouping = CanGrouping,
                ScheduleSettings = ScheduleSettings,
                ScheduleArguments = ScheduleArguments,
                JobTaskMetadata = new JobTaskMetadataExample(),
                RetrySettings = RetrySettings,
                NotifySettings = NotifySettings,
                Tags = Tags,
                GroupingSettings = GroupingSettings,
                ExecutionTarget = ExecutionTarget
            };
        }

        /// <summary>
        /// Проверить, может ли задача быть выполнена сейчас
        /// </summary>
        public bool CanExecuteNow()
        {
            return IsEnabled &&
                   !JobTaskMetadata.IsRunning &&
                   (AllowConcurrentExecution || !JobTaskMetadata.IsRunning) &&
                   (string.IsNullOrEmpty(ExecutionPath) || System.IO.File.Exists(ExecutionPath));
        }

        /// <summary>
        /// Получить статус задачи в виде строки
        /// </summary>
        public string GetStatusString()
        {
            if (JobTaskMetadata.IsRunning)
                return "Выполняется";

            if (!IsEnabled)
                return "Отключена";

            if (JobTaskMetadata.IsCompleted)
                return "Завершена";

            if (!string.IsNullOrEmpty(JobTaskMetadata.LastError))
                return "Ошибка";

            return "Активна";
        }

        /// <summary>
        /// Получить цвет статуса
        /// </summary>
        public string GetStatusColor()
        {
            if (JobTaskMetadata.IsRunning)
                return "#FF9800";

            if (!IsEnabled)
                return "#9E9E9E";

            if (JobTaskMetadata.IsCompleted)
                return "#4CAF50";

            if (!string.IsNullOrEmpty(JobTaskMetadata.LastError))
                return "#F44336";

            return "#2196F3";
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            return $"{Name} (Id: {Id}, Status: {GetStatusString()})";
        }

        #endregion
    }
}