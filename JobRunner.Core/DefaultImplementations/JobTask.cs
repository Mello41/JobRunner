using JobRunner.Core.Entities;
using System;
using System.Collections.Concurrent;

namespace JobRunner.Core.DefaultImplementations
{
    /// <summary>
    /// Создаваемая задача (в рамках программы JobRunner) - пример реализации
    /// </summary>
    public class JobTask : IJobTask
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public long? PID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ExecutionPath { get; set; } = string.Empty;

        #region Состояние выполнения задачи
        public DateTime StartRun { get; set; }
        public DateTime EndRun { get; set; }
        public bool IsEnabled { get; set; } = true;
        public bool IsRunning { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? LastRun { get; set; }
        public DateTime? NextRun { get; set; }
        #endregion

        public INotifySettings NotifySettings { get; set; } = new NotifySettings();


        public bool IsAsyncExecution { get; set; }
        public IScheduleSettings ScheduleSettings { get; set; } = new ScheduleSettings();
        public string LastError { get; set; } = string.Empty;
        public IScheduleArguments ScheduleArguments { get; set; } = new ScheduleArguments();

        /// <summary>
        /// Метки (задаются пользователем)
        /// </summary>
        public ConcurrentDictionary<Guid, ITag> Tags { get; set; } = new();

        public JobTask()
        {
            Id = Guid.NewGuid();

            var tagDB = new Tag
            {
                Id = Guid.NewGuid(),
                Name = "БД (индексация)",
                Color = "#F5F5F5", 
                UsageCount = 0,
                CanGrooping = true
            };
            var tagEmail = new Tag
            {
                Id = Guid.NewGuid(),
                Name = "Email рассылка",
                Color = "#F5F5F5", 
                UsageCount = 0,
                CanGrooping = true
            };

            Tags.TryAdd(tagDB.Id, tagDB);
            Tags.TryAdd(tagEmail.Id, tagEmail);
        }
    }
}
