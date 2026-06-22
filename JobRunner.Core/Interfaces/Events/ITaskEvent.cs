using JobRunner.Core.Interfaces.Entities.JobTaskSettings.NotifySettings;
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
        INotifySettings<TID> NotifySettings { get; set; }
    }
}
