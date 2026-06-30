using JobRunner.Core.Interfaces.Entities.JobTaskSettings;
using JobRunner.Core.Interfaces.Entities.JobTaskSettings.NotifySettings;
using JobRunner.Core.Models.Enums.NotificationEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace JobRunner.Core.DefaultImplementations
{
    /// <summary>
    /// Пример настроек уведомления задачи (с Generic)
    /// </summary>
    /// <typeparam name="TId">Тип идентификатора задачи</typeparam>
    public class NotifySettingsExample<TId> : INotifySettings<TId>
                                       where TId : IEquatable<TId>
    {
        /// <summary>
        /// Уведомлять до выполнения
        /// </summary>
        public bool NotifyBefore { get; set; }
        public bool EnableNotifications { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Dictionary<JobNotificationState, IJobStatusNotificationSettings<TId>> StatusSettings { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public TId JobTaskId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void AddRecipientToStatus(JobNotificationState state, INotificationRecipient<long> recipient)
        {
            throw new NotImplementedException();
        }

        public void AddRecipientToStatus(JobNotificationState state, INotificationRecipient<TId> recipient)
        {
            throw new NotImplementedException();
        }

        public IJobStatusNotificationSettings<TId> GetOrCreateStatusSettings(JobNotificationState state)
        {
            throw new NotImplementedException();
        }

        public List<INotificationRecipient<long>> GetRecipientsForStatus(JobNotificationState state)
        {
            throw new NotImplementedException();
        }

        public List<INotificationRecipient<long>> GetRecipientsForStatusAndMethod(JobNotificationState state, NotificationType method)
        {
            throw new NotImplementedException();
        }

        public IJobStatusNotificationSettings<TId>? GetStatusSettings(JobNotificationState state)
        {
            throw new NotImplementedException();
        }

        public bool RemoveRecipientFromStatus(JobNotificationState state, string recipientId)
        {
            throw new NotImplementedException();
        }

        List<INotificationRecipient<TId>> INotifySettings<TId>.GetRecipientsForStatus(JobNotificationState state)
        {
            throw new NotImplementedException();
        }

        List<INotificationRecipient<TId>> INotifySettings<TId>.GetRecipientsForStatusAndMethod(JobNotificationState state, NotificationType method)
        {
            throw new NotImplementedException();
        }
    }
}