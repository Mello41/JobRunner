using System;
using System.Collections.Generic;

namespace JobRunner.Core.DTO.Results.Group
{
    /// <summary>
    /// Предупреждение валидации группы
    /// </summary>
    /// <typeparam name="TId">Тип идентификатора задачи</typeparam>
    public class GroupValidationWarning<TId> where TId : IEquatable<TId>
    {
        /// <summary>
        /// Сообщение предупреждения
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// ID задач, связанных с предупреждением (опционально)
        /// </summary>
        public List<TId>? AffectedTaskIds { get; set; }
    }
}
