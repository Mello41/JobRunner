using JobRunner.Core.Interfaces.Entities.JobTaskSettings;
using System;

namespace JobRunner.Core.Interfaces.Events
{
    /// <summary>
    /// Базовый интерфейс для всех событий задач
    /// </summary>
    public interface ITaskEvent<TID> where TID : IEquatable<TID>
    {
        TID TaskId { get; set; }
        string TaskName { get; set; }
        INotifySettings NotifySettings { get; set; }
    }
}
