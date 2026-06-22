using JobRunner.Core.DTO.Results.Group;
using System;
using System.Collections.Generic;

namespace JobRunner.Core.DTO.Grouping
{
    /// <summary>
    /// Результат валидации группы задач
    /// </summary>
    /// <typeparam name="TId">Тип идентификатора задачи</typeparam>
    public class GroupValidationResult<TId> where TId : IEquatable<TId>
    {
        /// <summary>
        /// Группа валидна?
        /// </summary>
        public bool IsValid { get; set; } = true;

        /// <summary>
        /// Список ошибок (делают группу невалидной)
        /// </summary>
        public List<GroupValidationError<TId>> Errors { get; set; } = new();

        /// <summary>
        /// Список предупреждений (не блокируют выполнение)
        /// </summary>
        public List<GroupValidationWarning<TId>> Warnings { get; set; } = new();

        /// <summary>
        /// Создать успешный результат
        /// </summary>
        public static GroupValidationResult<TId> Success() => new() { IsValid = true };

        /// <summary>
        /// Создать результат с ошибками
        /// </summary>
        public static GroupValidationResult<TId> Fail(params GroupValidationError<TId>[] errors)
        {
            return new GroupValidationResult<TId>
            {
                IsValid = false,
                Errors = new List<GroupValidationError<TId>>(errors)
            };
        }

        /// <summary>
        /// Добавить ошибку
        /// </summary>
        public GroupValidationResult<TId> AddError(TId taskId, string message, string? taskName = null)
        {
            Errors.Add(new GroupValidationError<TId>
            {
                TaskId = taskId,
                Message = message,
                AffectedTaskName = taskName
            });
            IsValid = false;
            return this;
        }

        /// <summary>
        /// Добавить предупреждение
        /// </summary>
        public GroupValidationResult<TId> AddWarning(string message, List<TId>? taskIds = null)
        {
            Warnings.Add(new GroupValidationWarning<TId>
            {
                Message = message,
                AffectedTaskIds = taskIds
            });
            return this;
        }
    }
}
