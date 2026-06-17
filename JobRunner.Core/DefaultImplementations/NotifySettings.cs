using JobRunner.Core.Interfaces.Entities.JobTaskSettings;
using JobRunner.Core.Interfaces.Entities.JobTaskSettings.NotifySettings;
using JobRunner.Core.Models.Enums.NotificationEnums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JobRunner.Core.DefaultImplementations
{
    /// <summary>
    /// Пример настроек уведомления задачи (с Generic)
    /// </summary>
    /// <typeparam name="TUserId">Тип идентификатора пользователя</typeparam>
    public class NotifySettingsExample<TUserId> : INotifySettings<TUserId>
        where TUserId : IEquatable<TUserId>
    {
        #region Базовые настройки (INotifySettings)
        /// <summary>
        /// Уведомлять до выполнения
        /// </summary>
        public bool NotifyBefore { get; set; }

        /// <summary>
        /// За сколько времени до выполнения уведомить
        /// </summary>
        public TimeSpan NotifyBeforeTime { get; set; }

        /// <summary>
        /// Уведомлять сразу после старта
        /// </summary>
        public bool NotifyOnStarted { get; set; }

        /// <summary>
        /// За сколько времени после старта уведомить
        /// </summary>
        public TimeSpan NotifyOnStartedTime { get; set; }

        /// <summary>
        /// Уведомлять после выполнения
        /// </summary>
        public bool NotifyAfter { get; set; }

        /// <summary>
        /// За сколько времени после выполнения уведомить
        /// </summary>
        public TimeSpan NotifyAfterTime { get; set; }

        /// <summary>
        /// Текст уведомления
        /// </summary>
        public string NotificationMessage { get; set; } = string.Empty;

        /// <summary>
        /// Способы уведомления (можно несколько)
        /// </summary>
        public List<NotificationType> NotificationMethods { get; set; } = new();
        #endregion

        #region Получатели (INotifySettings<TUserId>)
        /// <summary>
        /// Список получателей (пользователей)
        /// </summary>
        public List<INotificationRecipient<TUserId>> Recipients { get; set; } = new();

        /// <summary>
        /// Добавить получателя
        /// </summary>
        public void AddRecipient(TUserId userId, List<NotificationType>? methods = null)
        {
            var existing = Recipients.FirstOrDefault(r => r.UserId.Equals(userId));
            if (existing != null)
            {
                existing.Methods = methods ?? existing.Methods;
                existing.IsEnabled = true;
                return;
            }
            /*
            Recipients.Add(new NotificationRecipientExample<TUserId>
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                Methods = methods,
                IsEnabled = true
            });*/
        }

        /// <summary>
        /// Удалить получателя
        /// </summary>
        public bool RemoveRecipient(TUserId userId)
        {
            var recipient = Recipients.FirstOrDefault(r => r.UserId.Equals(userId));
            if (recipient == null) return false;
            return Recipients.Remove(recipient);
        }

        /// <summary>
        /// Получить всех получателей для конкретного способа уведомления
        /// </summary>
        public List<TUserId> GetRecipientsForMethod(NotificationType method)
        {
            return Recipients
                .Where(r => r.IsEnabled && (r.Methods == null || r.Methods.Contains(method)))
                .Select(r => r.UserId)
                .Distinct()
                .ToList();
        }

        /// <summary>
        /// Получить контактные данные получателя
        /// </summary>
        public string? GetContactForRecipient(TUserId userId, NotificationType method)
        {
            // В реальном проекте здесь вызов сервиса пользователей
            // return await _userContactService.GetContactAsync(userId, method);

            // Для примера возвращаем заглушку
            return method switch
            {
                NotificationType.Email => $"user_{userId}@example.com",
               // NotificationType.Sms => $"+79990000{userId}",
                NotificationType.Telegram => $"telegram_{userId}",
                _ => null
            };
        }
        #endregion

        #region FormatMessage
        /// <summary>
        /// Форматирует сообщение уведомления с подстановкой параметров
        /// </summary>
        public string FormatMessage(string taskName, bool isSuccess, string? errorMessage = null)
        {
            if (!string.IsNullOrWhiteSpace(NotificationMessage))
            {
                var status = isSuccess ? "успешно" : "с ошибкой";
                var message = NotificationMessage
                    .Replace("{TaskName}", taskName) // , StringComparison.OrdinalIgnoreCase
                    .Replace("{Status}", status); // , StringComparison.OrdinalIgnoreCase

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    message = message.Replace("{ErrorMessage}", errorMessage); // , StringComparison.OrdinalIgnoreCase
                }

                return message;
            }

            return isSuccess
                ? $"Задача '{taskName}' успешно выполнена"
                : $"Задача '{taskName}' завершилась с ошибкой: {errorMessage}";
        }
        #endregion
    }
}