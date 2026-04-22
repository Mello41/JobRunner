namespace JobRunner.Core.Entities
{
    /// <summary>
    /// На всякий случай 
    /// (должна быть конечная реализация, но наследоваться от )
    /// </summary>
    public interface IScheduleArgumentItem
    {
        string Key { get; set; }
        string Value { get; set; }
        bool IsEncrypted { get; set; }
    }
}
