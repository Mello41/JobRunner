using JobRunner.Core.Entities.ValueObjects;
using JobRunner.Core.Interfaces.Scheduler;
using JobRunner.Quartz.Adapters;
using JobRunner.Quartz.Converters;
using Quartz;
using Quartz.Impl;

namespace JobRunner.Quartz.Scheduler
{
    /// <summary>
    /// Реализация IJobScheduler через Quartz.NET
    /// </summary>
    public class QuartzScheduler : IJobScheduler
    {
        private IScheduler _scheduler;
        private readonly CronConverter _cronConverter = new();

        /// <summary>
        /// Запускает планировщик Quartz.NET
        /// </summary>
        /// <returns></returns>
        public async Task StartProgramAsync(CancellationToken cancellationToken = default)
        {
            var factory = new StdSchedulerFactory();
            _scheduler = await factory.GetScheduler();
            await _scheduler.Start(cancellationToken);
        }

        /// <summary>
        /// Останавливает планировщик и освобождает ресурсы
        /// </summary>
        /// <returns></returns>
        public async Task StopProgramAsync(CancellationToken cancellationToken = default)
        {
            if (_scheduler != null && !_scheduler.IsShutdown)
                await _scheduler.Shutdown(cancellationToken);
        }

        /// <summary>
        /// Немедленный запуск задачи вне зависимости от расписания
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public async Task<bool> RunNowAsync(Guid taskId, CancellationToken cancellationToken = default)
        {
            await _scheduler.TriggerJob(new JobKey(taskId.ToString()), cancellationToken);
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
        public async Task<bool> PauseAsync(Guid taskId, CancellationToken cancellationToken = default)
        {
            await _scheduler.PauseJob(new JobKey(taskId.ToString()));
            return true;
        }

        /// <summary>
        /// Возобновляет выполнение ранее приостановленной задачи
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public async Task<bool> ResumeAsync(Guid taskId, CancellationToken cancellationToken = default)
        {
            await _scheduler.ResumeJob(new JobKey(taskId.ToString()));
            return true;
        }

        /// <summary>
        /// Прерывает выполнение текущего экземпляра задачи
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public async Task<bool> StopAsync(Guid taskId, CancellationToken cancellationToken = default)
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
        public async Task<bool> RestartAsync(Guid taskId, int delay = 100, CancellationToken cancellationToken = default)
        {
            await StopAsync(taskId, cancellationToken);
            await Task.Delay(delay, cancellationToken);
            return await RunNowAsync(taskId, cancellationToken);
        }

        /// <summary>
        /// Регистрация задачи в планировщике (вызывается оркестратором)
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="schedule"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task ScheduleAsync(Guid taskId, IScheduleSettings schedule, CancellationToken cancellationToken = default)
        {
            var cronExpression = _cronConverter.Convert(schedule);
            if (string.IsNullOrEmpty(cronExpression))
                throw new InvalidOperationException("Cron expression is empty or invalid");

            var job = JobBuilder.Create<JobAdapter>()
                .WithIdentity(taskId.ToString())
                .UsingJobData("TaskId", taskId.ToString())
                .StoreDurably()
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity($"{taskId}-trigger")
                .WithCronSchedule(cronExpression)
                .Build();

            await _scheduler.ScheduleJob(job, trigger, cancellationToken);
        }

        /// <summary>
        /// Удаление задачи из планировщика (вызывается оркестратором)
        /// </summary>
        public async Task UnscheduleAsync(Guid taskId, CancellationToken cancellationToken = default)
        {
            await _scheduler.DeleteJob(new JobKey(taskId.ToString()), cancellationToken);
        }
    }
}
