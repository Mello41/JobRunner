using System;
using System.Security.Cryptography;

namespace JobRunner.Core.DTO.Results.Group
{
    /// <summary>
    /// Ошибка валидации группы
    /// </summary>
    /// <typeparam name="TId">Тип идентификатора задачи</typeparam>
    public class GroupValidationError<TId> where TId : IEquatable<TId>
    {
        /// <summary>
        /// ID задачи, вызвавшей ошибку
        /// </summary>
        public TId TaskId { get; set; } = default!;

        /// <summary>
        /// Сообщение об ошибке
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Название задачи (опционально, для UI)
        /// </summary>
        public string? AffectedTaskName { get; set; }
    }
}
