using JobRunner.Core.DTO.User;
using JobRunner.Core.Models.Enums.NotificationEnums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JobRunner.Core.Interfaces.Services.UserServices
{
    /// <summary>
    /// Абстракция для получения контактных данных пользователей.
    /// Реализуется на сервере.
    /// </summary>
    /// <typeparam name="TUserId">Тип идентификатора пользователя</typeparam>
    public interface IUserContactService<TUserId> where TUserId : IEquatable<TUserId>
    {
        /// <summary>
        /// Получить контактную информацию пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<UserContactInfo<TUserId>?> GetUserContactAsync(
            TUserId userId,
            CancellationToken ct = default);

        /// <summary>
        /// Получить контактную информацию для нескольких пользователей
        /// </summary>
        /// <param name="userIds"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<IReadOnlyList<UserContactInfo<TUserId>>> GetUsersContactsAsync(
            IEnumerable<TUserId> userIds,
            CancellationToken ct = default);

        /// <summary>
        /// Получить контакт для конкретного способа уведомления
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="method"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<string?> GetContactForMethodAsync(
            TUserId userId,
            NotificationType method,
            CancellationToken ct = default);
    }
}
