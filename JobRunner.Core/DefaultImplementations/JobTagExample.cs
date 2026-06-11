using JobRunner.Core.Interfaces.Entities;
using System;

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
    }
}
