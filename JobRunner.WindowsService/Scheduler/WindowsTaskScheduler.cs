using JobRunner.Core;
using JobRunner.Core.Events;
using JobRunner.Core.Interfaces;
using JobRunner.Core.Settings;
using JobRunner.WindowsService.Converters;
using Microsoft.Win32.TaskScheduler;
using Task = System.Threading.Tasks.Task;
using WinTask = Microsoft.Win32.TaskScheduler.Task;

namespace JobRunner.WindowsService.Scheduler
{
    /// <summary>
    /// Реализация планировщика задач через
    /// Windows Task Scheduler (планировщик заданий)
    /// </summary>
    public class WindowsTaskScheduler : IJobScheduler
    {
        private readonly Dictionary<long, JobTask> _tasks = new();
        private readonly object _tasksLock = new();

        /// <summary>
        /// Событие выполнения задачи (уведомление UI)
        /// </summary>
        public event EventHandler<JobExecutionEventArgs>? JobExecuted;

        #region
        public Task StartProgramAsync()
        {
            return Task.CompletedTask;
        }

        public Task StopProgramAsync()
        {
            return Task.CompletedTask;
        }
        #endregion

        #region Задачи
        public async Task<string> CreateTaskAsync(JobTask task)
        {
            using (TaskService ts = new TaskService())
            {
                var td = ts.NewTask();
                td.RegistrationInfo.Description = task.Name;
                td.Principal.LogonType = TaskLogonType.InteractiveToken;

                var trigger = TriggerConverter.Convert(task.ScheduleSettings);
                if (trigger != null)
                    td.Triggers.Add(trigger);

                td.Actions.Add(new ExecAction(task.ExecutionPath, task.Arguments, null));

                ts.RootFolder.RegisterTaskDefinition(
                    GetTaskName(task.Id),
                    td,
                    TaskCreation.CreateOrUpdate,
                    null,
                    null,
                    TaskLogonType.InteractiveToken
                );
            }

            lock (_tasksLock)
            {
                _tasks[task.Id] = task;
            }

            return task.Id.ToString();
        }

        public async Task<bool> DeleteTaskAsync(long taskId)
        {
            using (TaskService ts = new TaskService())
            {
                ts.RootFolder.DeleteTask(GetTaskName(taskId), false);

                lock (_tasksLock)
                {
                    _tasks.Remove(taskId);
                }

                return true;
            }
        }

        public Task<JobTask?> GetTaskByIdAsync(long id)
        {
            lock (_tasksLock)
            {
                _tasks.TryGetValue(id, out var task);
                return Task.FromResult(task);
            }
        }

        public Task<JobTask?> GetTaskByPIDAsync(long pid)
        {
            lock (_tasksLock)
            {
                var task = _tasks.Values.FirstOrDefault(t => t.PID == pid);
                return Task.FromResult(task);
            }
        }

        public async Task<bool> StopTaskAsync(long taskId)
        {
            using (TaskService ts = new TaskService())
            {
                var task = ts.GetTask(GetTaskName(taskId));
                if (task != null)
                {
                    task.Stop();
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> UpdateTaskAsync(JobTask task)
        {
            await DeleteTaskAsync(task.Id);
            await CreateTaskAsync(task);
            return true;
        }

        public async Task<bool> PauseTaskAsync(long taskId)
        {
            using (TaskService ts = new TaskService())
            {
                var task = ts.GetTask(GetTaskName(taskId));
                if (task != null)
                {
                    task.Definition.Settings.Enabled = false;
                    task.RegisterChanges();
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> ResumeTaskAsync(long taskId)
        {
            using (TaskService ts = new TaskService())
            {
                var task = ts.GetTask(GetTaskName(taskId));
                if (task != null)
                {
                    task.Definition.Settings.Enabled = true;
                    task.RegisterChanges();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Принудительный немедленный запуск задачи вне
        /// зависимости от настроенного расписания
        /// (реализация WindowsService)
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public async Task<bool> TriggerNowAsync(long taskId)
        {
            using (TaskService ts = new TaskService())
            {
                var task = ts.GetTask(GetTaskName(taskId));
                if (task != null)
                {
                    task.Run();
                    return true;
                }
            }
            return false;
        }

        private string GetTaskName(long taskId) => $"JobRunner_{taskId}";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IReadOnlyList<JobTask>> GetAllTasksList()
        {
            var tasks = new List<JobTask>();

            using (TaskService ts = new TaskService())
            {
                var allTasks = ts.FindAllTasks(t => t.Name.StartsWith("JobRunner_"));

                foreach (var task in allTasks)
                {
                    var jobTask = ConvertToJobTask(task);
                    tasks.Add(jobTask);
                }
            }

            return tasks;
        }

        /// <summary>
        /// Взять задачу и конвертировать в 
        /// задачу для программы JobRunner
        /// </summary>
        /// <param name="task">Задача из планировщика задачи</param>
        /// <returns></returns>
        private JobTask ConvertToJobTask(WinTask task)
        {
            var jobTask = new JobTask
            {
                Id = ExtractIdFromTaskName(task.Name),
                Name = task.Definition.RegistrationInfo.Description ?? task.Name,
                ExecutionPath = task.Definition.Actions.OfType<ExecAction>().FirstOrDefault()?.Path ?? "",
                Arguments = task.Definition.Actions.OfType<ExecAction>().FirstOrDefault()?.Arguments ?? ""
            };

            // Конвертация триггера в ScheduleSettings (упрощённо)
            var trigger = task.Definition.Triggers
                .OfType<Trigger>().FirstOrDefault();
            if (trigger != null)
            {
                jobTask.ScheduleSettings = ConvertTriggerToScheduleSettings(trigger);
            }

            return jobTask;
        }

        /// <summary>
        /// Извлечение числового идентификатора задачи 
        /// из имени задачи в планировщике задач
        /// формат: "JobRunner_12345" - (чтобы было удобнее находить)
        /// </summary>
        /// <param name="taskName"></param>
        /// <returns></returns>
        private long ExtractIdFromTaskName(string taskName)
        {
            var parts = taskName.Split('_');
            if (parts.Length == 2 && long.TryParse(parts[1], out var id))
                return id;

            return DateTime.Now.Ticks;
        }

        /// <summary>
        /// Конвертация триггеров 
        /// </summary>
        /// <param name="trigger">Триггер задачи планировщика Windows</param>
        /// <returns></returns>
        private ScheduleSettings ConvertTriggerToScheduleSettings(Trigger trigger)
        {
            var settings = new ScheduleSettings();

            switch (trigger)
            {
                case DailyTrigger daily:
                    settings.PeriodType = PeriodType.EveryDaily;
                    settings.Hour = daily.StartBoundary.Hour;
                    settings.Minute = daily.StartBoundary.Minute;
                    break;

                case WeeklyTrigger weekly:
                    settings.PeriodType = PeriodType.EveryWeekly;
                    settings.Hour = weekly.StartBoundary.Hour;
                    settings.Minute = weekly.StartBoundary.Minute;
                    settings.WeeklyDay = (DayOfWeek)weekly.DaysOfWeek;
                    break;

                default:
                    settings.PeriodType = PeriodType.Once;
                    break;
            }

            return settings;
        }
        #endregion
    }
}
