using JobRunner.Core.Models.Enums.NotificationEnums;
using System;
using System.Collections.Generic;

namespace JobRunner.Core.Interfaces.Entities.JobTaskSettings.NotifySettings.GroupSettings
{
    /// <summary>
    /// Настройки уведомлений для группы задач
    /// </summary>
    /// <typeparam name="TId">Тип идентификатора</typeparam>
    public interface IGroupNotifySettings<TId> where TId : IEquatable<TId>
    {
        /// <summary>
        /// ID группы (метки)
        /// </summary>
        TId GroupId { get; set; }

        /// <summary>
        /// Уведомлять о старте группы
        /// </summary>
        bool NotifyOnGroupStart { get; set; }

        /// <summary>
        /// Уведомлять о завершении группы (успешно)
        /// </summary>
        bool NotifyOnGroupComplete { get; set; }

        /// <summary>
        /// Уведомлять об ошибке группы
        /// </summary>
        bool NotifyOnGroupFailure { get; set; }

        /// <summary>
        /// Уведомлять, если группа остановлена из-за ошибки
        /// </summary>
        bool NotifyOnGroupStopped { get; set; }

        /// <summary>
        /// Получатели уведомлений для группы
        /// </summary>
        List<INotificationRecipient<TId>> Recipients { get; set; }

        /// <summary>
        /// Шаблон сообщения для группы
        /// </summary>
        string? NotificationMessage { get; set; }

        /// <summary>
        /// Способы уведомления (переопределяют индивидуальные)
        /// </summary>
        List<NotificationType>? NotificationMethods { get; set; }

        /// <summary>
        /// Использовать настройки из задач (если true - берём из задач, false - групповые)
        /// </summary>
        bool UseTaskSettings { get; set; }
    }
}
