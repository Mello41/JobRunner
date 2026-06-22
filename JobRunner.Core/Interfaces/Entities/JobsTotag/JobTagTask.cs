using System;

namespace JobRunner.Core.Interfaces.Entities.JobsTotag
{
    /// <summary>
    /// Интерфейс связи между меткой и задачей (с порядком выполнения)
    /// </summary>
    /// <typeparam name="TTagId"></typeparam>
    /// <typeparam name="TTaskId"></typeparam>
    public interface IJobTagTask<TTagId, TTaskId>
                            where TTagId : IEquatable<TTagId>
                            where TTaskId : IEquatable<TTaskId>
    {
        /// <summary>
        /// ID метки
        /// </summary>
        TTagId TagId { get; set; }

        /// <summary>
        /// ID задачи
        /// </summary>
        TTaskId TaskId { get; set; }

        /// <summary>
        /// Порядок выполнения (приоритет). Чем меньше число — тем раньше выполняется.
        /// </summary>
        int Order { get; set; }

        /// <summary>
        /// Дополнительные настройки для этой конкретной задачи в группе
        /// </summary>
        string? CustomSettings { get; set; }
    }
}
