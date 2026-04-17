using JobRunner.Core.Notify;
using JobRunner.Core.Settings;
using System;

namespace JobRunner.Core
{
    /// <summary>
    /// Создаваемая задача
    /// </summary>
    public class JobTask
    {
        public long Id { get; private set; }
        public long PID { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public string ExecutionPath { get; set; } = string.Empty;

        #region Состояние выполнения задачи
        public DateTime StartRun { get; set; }
        public DateTime EndRun { get; set; }

        /// <summary>
        /// задача активна? (вкл/выкл)
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        public bool IsCompleted { get; set; }

        public DateTime? LastRun { get; set; }
        public DateTime? NextRun { get; set; }
        #endregion

        #region Уведомления
        public TimeSpan TimeNotifyBeforeEnd { get; set; }
        public bool IsNotifyBeforeEnd { get; set; }
        #endregion

        /// <summary>
        /// Параллельное выполнение? 
        /// </summary>
        public bool IsAsyncExecution { get; set; }

        #region настройки задачи
        public ScheduleSettings ScheduleSettings { get; set; }
        public NotifySettings NotifySettings { get; set; }
        #endregion

        /// <summary>
        /// последняя ошибка
        /// </summary>
        public string LastError { get; set; } = string.Empty;

        #region Шифрование
        public string EncryptedArguments { get; set; }

        /// <summary>
        /// Используется ли шифрование? (для чувствительных данных)
        /// </summary>
        public bool IsEncrypt { get; set; }
        #endregion

        /// <summary>
        /// ctor    
        /// </summary>
        public JobTask()
        {
            Id = DateTime.Now.Ticks; // Id (private set) --> null
                                     // DateTime.Now.Ticks - это количество 100-наносекундных интервалов с 1 января 0001 года

            ScheduleSettings = new ScheduleSettings();
            NotifySettings = new NotifySettings();
        }
    }
}
