using System;

namespace JobRunner.Core.Interfaces.Events
{
    /// <summary>
    /// Базовые данные события задачи
    /// </summary>
    public interface ITaskEventData
    {
        Guid TaskId { get; set; }
        string TaskName { get; set; }
    }
}