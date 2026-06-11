using JobRunner.Core.Entities.ValueObjects;
using JobRunner.Core.Interfaces.Entities.Export;
using JobRunner.Core.Interfaces.Entities.JobTaskSettings;
using System;
using System.Collections.Generic;

namespace JobRunner.Core.DefaultImplementations
{
    public class JobTaskExportExample<TId> : IJobTaskExport<TId> where TId : IEquatable<TId>
    {
        public string Version { get; set; } = "1.0";
        public DateTime ExportedAt { get; set; } = DateTime.UtcNow;
        public string ExportedBy { get; set; } = Environment.UserName;
        public string? Description { get; set; }

        public IJobTaskBasicInfo<TId> BasicInfo { get; set; } = new JobTaskBasicInfoExample<TId>();
        public IScheduleSettings ScheduleSettings { get; set; } = new DailySchedule();
        public INotifySettings NotifySettings { get; set; } //= new NotifySettingsExample();
        public IScheduleArguments ScheduleArguments { get; set; } = new ScheduleArgumentsExample();
        public IRetrySettings RetrySettings { get; set; } = new RetrySettingsExample();
        public List<TId> Tags { get; set; } = new();
        public Dictionary<string, string>? Metadata { get; set; }
    }
}
