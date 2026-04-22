using JobRunner.Core.Entities;
using System;

namespace JobRunner.Core.DefaultImplementations
{
    public class Tag : ITag
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public int UsageCount { get; set; }
        public bool CanGrooping { get; set; }
    }
}
