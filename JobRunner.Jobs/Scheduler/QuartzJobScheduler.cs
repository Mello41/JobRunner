using JobRunner.Core.DefaultImplementations;
using JobRunner.Core.Interfaces;
using JobRunner.Core.Settings;
using JobRunner.Jobs.Adapters;
using Quartz;
using Quartz.Impl;
using System.Diagnostics;

namespace JobRunner.Jobs.Scheduler
{
    public class QuartzJobScheduler : IJobScheduler
    {
        private IScheduler _scheduler;
        private readonly IStringEncryptor _encryptor;

        private readonly Dictionary<long, JobTask> _tasks = new();

        public QuartzJobScheduler(IStringEncryptor encryptor)
        {
            _encryptor = encryptor;
        }

        public async Task StartProgramAsync()
        {
            var factory = new StdSchedulerFactory();
            _scheduler = await factory.GetScheduler();
            await _scheduler.Start();
        }

        public async Task StopProgramAsync()
        {
            if (_scheduler != null && !_scheduler.IsShutdown)
                await _scheduler.Shutdown();
        }

        public async Task<string> CreateTaskAsync(JobTask task)
        {
            if (task.Id == 0)
            {
                var property = typeof(JobTask).GetProperty("Id");
                property?.SetValue(task, DateTime.Now.Ticks);
            }

            var job = JobBuilder.Create<JobAdapter>()
                .WithIdentity(task.Id.ToString())
                .UsingJobData("TaskId", task.Id.ToString())
                .Build();

            var trigger = BuildTrigger(task);

            await _scheduler.ScheduleJob(job, trigger);

            _tasks[task.Id] = task;

            return task.Id.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteTaskAsync(long taskId)
        {
            var taskIdStr = taskId.ToString();

            await _scheduler.DeleteJob(new JobKey(taskIdStr));

            return _tasks.Remove(taskId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<IReadOnlyList<JobTask>> GetAllTasksList()
        {
            return Task.FromResult<IReadOnlyList<JobTask>>(
                new List<JobTask>(_tasks.Values));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<JobTask?> GetTaskByIdAsync(long id)
        {
            _tasks.TryGetValue(id, out var task);
            return Task.FromResult(task);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public Task<JobTask?> GetTaskByPIDAsync(long pid)
        {
            foreach (var task in _tasks.Values)
            {
                if (task.PID == pid)
                    return Task.FromResult<JobTask?>(task);
            }
            return Task.FromResult<JobTask?>(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public async Task<bool> StopTaskAsync(long taskId)
        {
            if (!_tasks.TryGetValue(taskId, out var task))
                return false;

            if (task.PID > 0)
            {
                try
                {
                    var process = Process.GetProcessById((int)task.PID);
                    process.Kill();
                    await process.WaitForExitAsync();
                    task.PID = 0;
                    task.IsRunning = false;
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public async Task<bool> UpdateTaskAsync(JobTask task)
        {
            if (!_tasks.ContainsKey(task.Id))
                return false;

            _tasks[task.Id] = task;

            var taskIdStr = task.Id.ToString();
            await _scheduler.DeleteJob(new JobKey(taskIdStr));

            var job = JobBuilder.Create<JobAdapter>()
                .WithIdentity(taskIdStr)
                .UsingJobData("TaskId", taskIdStr)
                .Build();

            var trigger = BuildTrigger(task);
            await _scheduler.ScheduleJob(job, trigger);

            return true;
        }

        /// <summary>
        /// Создание триггера Quartz на основе настроек расписания задачи
        /// Триггер - часть полного расписания для задачи
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        private ITrigger BuildTrigger(JobTask task)
        {
            var triggerBuilder = TriggerBuilder.Create()
                .WithIdentity($"{task.Id}-trigger");

            if (task.ScheduleSettings != null)
            {
                var cronExpression = ConvertToCron(task.ScheduleSettings);
                if (!string.IsNullOrEmpty(cronExpression))
                {
                    triggerBuilder.WithCronSchedule(cronExpression);
                }
            }

            return triggerBuilder.Build();
        }

        /// <summary>
        /// Конвертация к виду Cron (с учетом настроек "ScheduleSettings")
        /// </summary>
        /// <param name="settings">ScheduleSettings класс с настройками задачи</param>
        /// <returns></returns>
        private string ConvertToCron(ScheduleSettings settings)
        {
            return settings.PeriodType switch
            {
                PeriodType.Once => string.Empty,
                PeriodType.EveryMinutes => $"0 */{settings.IntervalMinutes} * * * ?",
                PeriodType.EveryHourly => $"0 {settings.Minute} * * * ?",
                PeriodType.EveryDaily => $"0 {settings.Minute} {settings.Hour} * * ?",
                PeriodType.EveryWeekly => $"0 {settings.Minute} {settings.Hour} ? * {(int)settings.WeeklyDay + 1}",
                PeriodType.EveryMonthly => $"0 {settings.Minute} {settings.Hour} {settings.MonthDay} * ?",
                PeriodType.EveryQuarterly => $"0 {settings.Minute} {settings.Hour} {settings.MonthDay} 1,4,7,10 ?",
                PeriodType.EveryYearly => $"0 {settings.Minute} {settings.Hour} {settings.MonthDay} 1 ?",
                _ => string.Empty
            };
        }

        public async Task<bool> TriggerNowAsync(long taskId)
        {
            await _scheduler.TriggerJob(new JobKey(taskId.ToString()));
            return true;
        }

        public async Task<bool> ResumeTaskAsync(long taskId)
        {
            await _scheduler.ResumeJob(new JobKey(taskId.ToString()));
            return true;
        }
    }
}
