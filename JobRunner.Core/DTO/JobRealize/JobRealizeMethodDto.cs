using System.Collections.Generic;

namespace JobRunner.Core.DTO.JobRealize
{
    public class JobRealizeMethodDto
    {
        public string Type { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public string FullPathName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public bool IsEnabled { get; set; }
        public string? Description { get; set; }
        public List<FixedArgumentDto> FixedArguments { get; set; } = new();
    }
}
