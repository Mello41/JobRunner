using JobRunner.Core.Entities;
using JobRunner.Core.Interfaces;
using JobRunner.Quartz.Adapters;
using JobRunner.Quartz.Converters;
using Quartz;
using Quartz.Impl;

namespace JobRunner.Quartz.Scheduler
{
    /// <summary>
    /// Реализация IJobScheduler через Quartz.NET
    /// </summary>
    public class QuartzScheduler<T> : IJobScheduler<T> where T : IJobTask
    {
        private IScheduler _scheduler;
        private readonly CronConverter _cronConverter = new();

        /// <summary>
        /// Запускает планировщик Quartz.NET
        /// </summary>
        /// <returns></returns>
        public async Task StartProgramAsync()
        {
            var factory = new StdSchedulerFactory();
            _scheduler = await factory.GetScheduler();
            await _scheduler.Start();
        }

        /// <summary>
        /// Останавливает планировщик и освобождает ресурсы
        /// </summary>
        /// <returns></returns>
        public async Task StopProgramAsync()
        {
            if (_scheduler != null && !_scheduler.IsShutdown)
                await _scheduler.Shutdown();
        }

        /// <summary>
        /// Немедленный запуск задачи вне зависимости от расписания
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public async Task<bool> RunNowAsync(Guid taskId)
        {
            await _scheduler.TriggerJob(new JobKey(taskId.ToString()));
            return true;
        }

        /// <summary>
        /// Приостанавливает выполнение задачи по расписанию
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        /// <remarks>
        /// Задача не будет запускаться по триггерам до вызова ResumeAsync()
        /// </remarks>
        public async Task<bool> PauseAsync(Guid taskId)
        {
            await _scheduler.PauseJob(new JobKey(taskId.ToString()));
            return true;
        }

        /// <summary>
        /// Возобновляет выполнение ранее приостановленной задачи
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public async Task<bool> ResumeAsync(Guid taskId)
        {
            await _scheduler.ResumeJob(new JobKey(taskId.ToString()));
            return true;
        }

        /// <summary>
        /// Прерывает выполнение текущего экземпляра задачи
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public async Task<bool> StopAsync(Guid taskId)
        {
            await _scheduler.Interrupt(new JobKey(taskId.ToString()));
            return true;
        }

        /// <summary>
        /// Перезапускает задачу (останавливает и запускает заново)
        /// </summary>
        /// <param name="taskId">Guid задачи</param>
        /// <param name="delay">предполагаемая задержка для перезапуска 
        /// (дефолт --> 100 мс)</param>
        /// <returns></returns>
        public async Task<bool> RestartAsync(Guid taskId, int delay = 100)
        {
            await StopAsync(taskId);
            await Task.Delay(delay); 
            return await RunNowAsync(taskId);
        }

        /// <summary>
        /// Регистрация задачи в планировщике (вызывается оркестратором)
        /// </summary>
        internal async Task ScheduleAsync(T task)
        {
            var cronExpression = _cronConverter.Convert(task.ScheduleSettings);
            if (string.IsNullOrEmpty(cronExpression))
                return;

            var job = JobBuilder.Create<JobAdapter>()
                .WithIdentity(task.Id.ToString())
                .UsingJobData("TaskId", task.Id.ToString())
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity($"{task.Id}-trigger")
                .WithCronSchedule(cronExpression)
                .Build();

            await _scheduler.ScheduleJob(job, trigger);
        }

        /// <summary>
        /// Удаление задачи из планировщика (вызывается оркестратором)
        /// </summary>
        internal async Task UnscheduleAsync(Guid taskId)
        {
            await _scheduler.DeleteJob(new JobKey(taskId.ToString()));
        }
    }
}
