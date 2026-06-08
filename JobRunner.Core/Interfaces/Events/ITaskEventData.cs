using System;

namespace JobRunner.Core.Interfaces.Events
{
    /// <summary>
    /// Базовые данные события задачи
    /// </summary>
    /// <typeparam name="TId">Тип идентификатора задачи</typeparam>
    public interface ITaskEventData<TId> where TId : IEquatable<TId>
    {
        /// <summary>
        /// Универсальный тип ID
        /// </summary>
        TId TaskId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string TaskName { get; set; }
    }
}