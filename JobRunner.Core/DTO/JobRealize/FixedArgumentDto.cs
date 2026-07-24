namespace JobRunner.Core.DTO.JobRealize
{
    public class FixedArgumentDto
    {
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public bool IsEncrypted { get; set; }
        public bool IsRequired { get; set; }
        public string? Description { get; set; }
    }
}
