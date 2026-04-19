using JobRunner.Core;
using JobRunner.Core.Events;
using JobRunner.Core.Interfaces;
using Microsoft.Win32.TaskScheduler;
using Task = System.Threading.Tasks.Task;
using JobRunner.WindowsService.Converters;

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

        public Task StartProgramAsync()
        {
            return Task.CompletedTask;
        }

        public Task StopProgramAsync()
        {
            return Task.CompletedTask;
        }

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

        public Task<IReadOnlyList<JobTask>> GetAllTasksList()
        {
            lock (_tasksLock)
            {
                return Task.FromResult<IReadOnlyList<JobTask>>(_tasks.Values.ToList());
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
    }
}
