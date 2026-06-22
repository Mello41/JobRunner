using JobRunner.Core.Interfaces.Entities;
using JobRunner.Core.Interfaces.Entities.JobTaskSettings;
using System;
using System.Collections.Generic;

namespace JobRunner.Core.DefaultImplementations
{
    /// <summary>
    /// Пример реализации ITag
    /// </summary>
    public class JobTagExample : IJobTag<Guid>
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string? Color { get; set; } = "#888888";
        public bool CanGrouping { get; set; }
        public string Description { get; set; }
        public bool AllowGroupRun { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool AllowGroupPause { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public SortedSet<Guid>? OrderedTaskIds { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IGroupingSettings GroupingSettings { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
