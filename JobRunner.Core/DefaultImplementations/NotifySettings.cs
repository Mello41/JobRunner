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
        /// <summary>
        /// Уведомлять до выполнения
        /// </summary>
        public bool NotifyBefore { get; set; }
        public bool EnableNotifications { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Dictionary<JobNotificationState, IJobStatusNotificationSettings> StatusSettings { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void AddRecipientToStatus(JobNotificationState state, INotificationRecipient<long> recipient)
        {
            throw new NotImplementedException();
        }

        public IJobStatusNotificationSettings GetOrCreateStatusSettings(JobNotificationState state)
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

        public IJobStatusNotificationSettings? GetStatusSettings(JobNotificationState state)
        {
            throw new NotImplementedException();
        }

        public bool RemoveRecipientFromStatus(JobNotificationState state, string recipientId)
        {
            throw new NotImplementedException();
        }
    }
}