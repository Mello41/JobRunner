using System;
using System.Collections.Concurrent;

namespace JobRunner.Core.DefaultImplementations
{
    /// <summary>
    /// Создаваемая задача (в рамках программы JobRunner) - пример реализации
    /// </summary>
    public class JobTask
    {
        /// <summary>
        /// ID задачи
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Process ID задачи 
        /// (существует только тогда, когда задача запущена)
        /// </summary>
        public long? PID { get; set; }

        /// <summary>
        /// наименование задачи (обычное пользовательское, 
        /// которое конвертируется в формат "JobRunner_..." 
        /// в Win планировщике задач)
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Описание задачи
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Путь к исполняемому файлу задачи
        /// </summary>
        public string ExecutionPath { get; set; } = string.Empty;

        #region Состояние выполнения задачи
        /// <summary>
        /// Дата начала выполнения задачи
        /// </summary>
        public DateTime StartRun { get; set; }

        /// <summary>
        /// Дата окончания действия задачи
        /// </summary>
        public DateTime EndRun { get; set; }

        /// <summary>
        /// задача активна? (вкл/выкл)
        /// </summary>
        public bool IsEnabled { get; set; } = true;
        
        /// <summary>
        /// Задача запущена сейчас?
        /// </summary>
        public bool IsRunning { get; set; } 

        /// <summary>
        /// Задача завершена?
        /// </summary>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// Дата последнего запуска
        /// </summary>
        public DateTime? LastRun { get; set; }

        /// <summary>
        /// Дата следующего запуска
        /// </summary>
        public DateTime? NextRun { get; set; }
        #endregion

        #region Уведомления

        /// <summary>
        /// Уведомления задачи (настраиваемые) - раз в промежуток (какой)
        /// </summary>
        public NotifySettings NotifySettings { get; set; }

        /// <summary>
        /// Время, которое закладывается перед 
        /// выполнением задачи для уведомления
        /// </summary>
        public TimeSpan TimeNotifyBeforeEnd { get; set; }

        /// <summary>
        /// Время, через которое уведомить о выполнении задачи
        /// </summary>
        public TimeSpan TimeNotifyAfterEnd { get; set; }

        /// <summary>
        /// Уведомить перед завершением?
        /// </summary>
        public bool IsNotifyBeforeEnd { get; set; }

        /// <summary>
        /// Уведомить после завершения?
        /// </summary>
        public bool IsNotifyAfterEnd { get; set; }
        #endregion

        /// <summary>
        /// Параллельное выполнение? 
        /// </summary>
        public bool IsAsyncExecution { get; set; }

        /// <summary>
        /// Настройки конкретики периодичности (указание минут, часов и тд)
        /// </summary>
        public ScheduleSettings ScheduleSettings { get; set; }

        /// <summary>
        /// последняя ошибка
        /// </summary>
        public string LastError { get; set; } = string.Empty;

        /// <summary>
        /// Аргументы у задачи (с возможность шифрования)
        /// </summary>
        public ScheduleArguments ScheduleArguments { get; set; }

        /// <summary>
        /// ctor
        /// </summary>
        public JobTask()
        {
            Id = Guid.NewGuid(); 
                                     
            ScheduleSettings = new ScheduleSettings();
            NotifySettings = new NotifySettings();
            ScheduleArguments = new ScheduleArguments();

            #region Метки (по умолчанию)
            Tags = new ConcurrentDictionary<string, byte>();
            Tags.TryAdd("Thumbnails", 0);
            Tags.TryAdd("БД (индексация)", 0);
            #endregion
        }

        /// <summary>
        /// Метки (задаются пользователем)
        /// </summary>
        public ConcurrentDictionary<string, byte> Tags { get; set; } = new ConcurrentDictionary<string, byte>();
    }
}
