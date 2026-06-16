using JobRunner.Core.Interfaces.Entities.Export;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace JobRunner.Core.Interfaces.Services.EntityServices
{
    /// <summary>
    /// Сервис для управления настройками и аргументами задачи
    /// </summary>
    /// <typeparam name="TId">Тип идентификатора задачи</typeparam>
    public interface IScheduleSettingsService<TId> 
                        where TId : IEquatable<TId>
    {
        /// <summary>
        /// Получить все аргументы задачи
        /// </summary>
        /// <param name="scheduleId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<Dictionary<string, object>> GetAllArgumentsAsync(TId scheduleId, CancellationToken ct = default);

        /// <summary>
        /// Получить аргумент по ключу
        /// </summary>
        /// <param name="scheduleId"></param>
        /// <param name="key"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<object?> GetArgumentAsync(TId scheduleId, string key, CancellationToken ct = default);

        /// <summary>
        /// Установить значение аргумента
        /// </summary>
        /// <param name="scheduleId"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="isEncrypted"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<bool> SetArgumentAsync(TId scheduleId, string key, object value, bool isEncrypted = false, CancellationToken ct = default);

        /// <summary>
        /// Установить несколько аргументов
        /// </summary>
        /// <param name="scheduleId"></param>
        /// <param name="arguments"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<bool> SetArgumentsAsync(TId scheduleId, Dictionary<string, object> arguments, CancellationToken ct = default);

        /// <summary>
        /// Удалить аргумент
        /// </summary>
        /// <param name="scheduleId"></param>
        /// <param name="key"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<bool> RemoveArgumentAsync(TId scheduleId, string key, CancellationToken ct = default);

        /// <summary>
        /// Очистить все аргументы
        /// </summary>
        /// <param name="scheduleId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<bool> ClearAllArgumentsAsync(TId scheduleId, CancellationToken ct = default);

        /// <summary>
        /// Очистить конкретный аргумент (обнулить значение)
        /// </summary>
        /// <param name="scheduleId"></param>
        /// <param name="key"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<bool> ClearArgumentAsync(TId scheduleId, string key, CancellationToken ct = default);

        #region Экспорт/Импорт
        /// <summary>
        /// Экспортировать настройки задачи в JSON
        /// </summary>
        /// <param name="scheduleId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<string> ExportToJsonAsync(TId scheduleId, CancellationToken ct = default);

        /// <summary>
        /// Экспортировать настройки задачи в файл
        /// </summary>
        /// <param name="scheduleId"></param>
        /// <param name="filePath"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task ExportToJsonFileAsync(TId scheduleId, string filePath, CancellationToken ct = default);

        /// <summary>
        /// Импортировать настройки задачи из JSON
        /// </summary>
        /// <param name="json"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<IJobTaskExport<TId>> ImportFromJsonAsync(string json, CancellationToken ct = default);

        /// <summary>
        /// Импортировать настройки задачи из файла
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<IJobTaskExport<TId>> ImportFromJsonFileAsync(string filePath, CancellationToken ct = default);

        /// <summary>
        /// Применить импортированные настройки к существующей задаче
        /// </summary>
        /// <param name="scheduleId"></param>
        /// <param name="export"></param>
        /// <param name="overwriteArguments"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<bool> ApplyImportedSettingsAsync(TId scheduleId, IJobTaskExport<TId> export, bool overwriteArguments = true, CancellationToken ct = default);
        #endregion

        /// <summary>
        /// Валидация аргументов задачи
        /// </summary>
        /// <param name="scheduleId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<ValidationResult> ValidateArgumentsAsync(TId scheduleId, CancellationToken ct = default);
    }
}
