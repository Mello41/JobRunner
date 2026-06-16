using JobRunner.Core.Entities.Enums;
using JobRunner.Core.Models.Enums;
using System;
using System.Collections.Generic;

namespace JobRunner.Core.Interfaces.Services.EntityServices
{
    /// <summary>
    /// Настройки уведомлений для конкретного состояния задачи
    /// </summary>
    /// <typeparam name="TId">Тип идентификатора задачи</typeparam>
    public interface IJobNotificationSettings<TId> where TId : IEquatable<TId>
    {
        /// <summary>
        /// ID задачи
        /// </summary>
        TId JobTaskId { get; set; }

        /// <summary>
        /// Состояние задачи
        /// </summary>
        JobNotificationState State { get; set; }

        /// <summary>
        /// Включены ли уведомления для этого состояния
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Использовать настройки из NotifySettings задачи
        /// Если true — берём из JobTask.NotifySettings
        /// Если false — используем переопределённые настройки
        /// </summary>
        bool UseTaskSettings { get; set; }

        /// <summary>
        /// Переопределить текст уведомления
        /// </summary>
        string? CustomMessage { get; set; }

        /// <summary>
        /// Переопределить способы уведомления
        /// </summary>
        List<NotificationType>? CustomMethods { get; set; }

        /// <summary>
        /// Переопределить Email
        /// </summary>
        string? CustomEmail { get; set; }

        /// <summary>
        /// Переопределить Telegram Chat ID
        /// </summary>
        string? CustomTelegramChatId { get; set; }

        /// <summary>
        /// Переопределить Webhook URL
        /// </summary>
        string? CustomWebhookUrl { get; set; }

        /// <summary>
        /// Задержка перед отправкой (секунды)
        /// </summary>
        int DelaySeconds { get; set; }

        /// <summary>
        /// Отправлять только при первом возникновении состояния
        /// </summary>
        bool OnlyFirstTime { get; set; }

        /// <summary>
        /// Не отправлять, если задача выполняется вручную
        /// </summary>
        bool SkipManual { get; set; }
    }
}
