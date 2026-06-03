using JobRunner.Core.Entities;
using System;

namespace JobRunner.Core.DefaultImplementations
{
    /// <summary>
    /// Пример реализации ITag
    /// </summary>
    public class Tag : ITag
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string? Color { get; set; } = "#888888";
        public bool CanGrouping { get; set; }
        public string Description { get; set; }
    }
}
