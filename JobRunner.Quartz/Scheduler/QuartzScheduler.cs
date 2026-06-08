using JobRunner.Core.Entities;
using JobRunner.Core.Entities.ValueObjects;
using JobRunner.Core.Interfaces.Converters;
using JobRunner.Core.Interfaces.Scheduler;
using JobRunner.Quartz.Adapters;
using Quartz;

namespace JobRunner.Quartz.Scheduler
{
    /// <summary>
    /// Реализация IJobScheduler через Quartz.NET
    /// </summary>
    public class QuartzScheduler<TTask, TId> : IJobScheduler<TId>
                                where TTask : class, IJobTask<TId>
                                where TId : IEquatable<TId>
    {
        private readonly IScheduler _scheduler;
        private readonly IScheduleConverter _converter;

        public QuartzScheduler(IScheduler scheduler, IScheduleConverter converter)
        {
            _scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
            _converter = converter;
        }

        /// <summary>
        /// Запускает планировщик Quartz.NET
        /// </summary>
        /// <returns></returns>
        public async Task StartProgramAsync(CancellationToken cancellationToken = default)
        {
            if (!_scheduler.IsStarted)
                await _scheduler.Start(cancellationToken);
        }

        /// <summary>
        /// Останавливает планировщик и освобождает ресурсы
        /// </summary>
        /// <returns></returns>
        public async Task StopProgramAsync(CancellationToken cancellationToken = default)
        {
            if (!_scheduler.IsShutdown)
                await _scheduler.Shutdown(cancellationToken);
        }

        /// <summary>
        /// Немедленный запуск задачи вне зависимости от расписания
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public async Task<bool> RunNowAsync(TId taskId, CancellationToken cancellationToken = default)
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
        public async Task<bool> PauseAsync(TId taskId, CancellationToken cancellationToken = default)
        {
            await _scheduler.PauseJob(new JobKey(taskId.ToString()));
            return true;
        }

        /// <summary>
        /// Возобновляет выполнение ранее приостановленной задачи
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public async Task<bool> ResumeAsync(TId taskId, CancellationToken cancellationToken = default)
        {
            await _scheduler.ResumeJob(new JobKey(taskId.ToString()));
            return true;
        }

        /// <summary>
        /// Прерывает выполнение текущего экземпляра задачи
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public async Task<bool> StopAsync(TId taskId, CancellationToken cancellationToken = default)
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
        public async Task<bool> RestartAsync(TId taskId, int delay = 100, CancellationToken cancellationToken = default)
        {
            await StopAsync(taskId, cancellationToken);
            await Task.Delay(delay, cancellationToken);
            return await RunNowAsync(taskId, cancellationToken);
        }

        #region Управление состояниями задачи через Quartz
        /// <summary>
        /// Регистрация задачи в планировщике (вызывается оркестратором)
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="schedule"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task ScheduleAsync(TId taskId, IScheduleSettings schedule, CancellationToken cancellationToken = default)
        {
            var cronExpression = _converter.Convert(schedule);
            if (string.IsNullOrEmpty(cronExpression))
                throw new InvalidOperationException("Cron expression is empty or invalid");

            var job = JobBuilder.Create<JobAdapter<TTask, TId>>()
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
        /// Удалить задачу из планировщика
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UnscheduleAsync(TId taskId, CancellationToken cancellationToken = default)
        {
            await _scheduler.DeleteJob(new JobKey(taskId.ToString()), cancellationToken);
        }

        /// <summary>
        /// Обновить расписание задачи
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task RescheduleAsync(TId taskId, IScheduleSettings settings, CancellationToken cancellationToken = default)
        {
            await UnscheduleAsync(taskId, cancellationToken);
            await ScheduleAsync(taskId, settings, cancellationToken);
        }

        #endregion

        /// <summary>
        /// Перерегистрировать все задачи из БД (восстановление после перезапуска)
        /// </summary>
        /// <param name="tasks"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task RestoreSchedulesAsync(IEnumerable<IJobTask<TId>> tasks, CancellationToken ct = default)
        {
            foreach (var task in tasks)
            {
                if (!task.IsEnabled) continue;

                await ScheduleAsync(task.Id, task.ScheduleSettings, ct);
            }
        }
    }
}
