using JobRunner.Core.Interfaces.Converters;
using JobRunner.Core.Interfaces.Entities;
using JobRunner.Core.Interfaces.Entities.JobTaskSettings.ScheduleSettings;
using JobRunner.Core.Interfaces.Scheduler;
using JobRunner.Quartz.Adapters;
using JobRunner.Quartz.Extensions;
using Quartz;
using Quartz.Impl.Matchers;

namespace JobRunner.Quartz.Scheduler
{
    /// <summary>
    /// Реализация IJobScheduler через Quartz.NET
    /// </summary>
    /// <typeparam name="TTask">Тип задачи</typeparam>
    /// <typeparam name="TId">Тип идентификатора</typeparam>
    public class QuartzScheduler<TTask, TId> : IJobTaskScheduler<TId>
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
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task StartProgramAsync(CancellationToken cancellationToken = default)
        {
            if (!_scheduler.IsStarted)
                await _scheduler.Start(cancellationToken);
        }

        /// <summary>
        /// Останавливает планировщик и освобождает ресурсы
        /// </summary>
        /// <param name="cancellationToken"></param>
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
        /// <param name="cancellationToken"></param>
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
        /// <param name="cancellationToken"></param>
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
        /// <param name="cancellationToken"></param>
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
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> StopAsync(TId taskId, CancellationToken cancellationToken = default)
        {
            await _scheduler.Interrupt(new JobKey(taskId.ToString()));
            return true;
        }

        /// <summary>
        /// Перезапускает задачу (останавливает и запускает заново)
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="delay"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> RestartAsync(TId taskId, int delay = 100, CancellationToken cancellationToken = default)
        {
            await StopAsync(taskId, cancellationToken);
            await Task.Delay(delay, cancellationToken);
            return await RunNowAsync(taskId, cancellationToken);
        }

        #region Управление расписанием

        /// <summary>
        /// Регистрация задачи в планировщике (вызывается оркестратором)
        /// </summary>
        /// <exception cref="InvalidOperationException">Когда Cron выражение пустое или невалидное</exception>
        public async Task ScheduleAsync(TId taskId, IScheduleSettings schedule, CancellationToken cancellationToken = default)
        {
            var cronExpression = _converter.Convert(schedule);

            cronExpression.ValidateCron();

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
        /// <param name="settings"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task RescheduleAsync(TId taskId, IScheduleSettings settings, CancellationToken cancellationToken = default)
        {
            await UnscheduleAsync(taskId, cancellationToken);
            await ScheduleAsync(taskId, settings, cancellationToken);
        }

        #endregion

        /// <summary>
        /// Перерегистрировать все задачи 
        /// из БД (восстановление после перезапуска)
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

        /// <summary>
        /// Проверить, существует ли задача в планировщике
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<bool> JobExistsAsync(TId taskId, CancellationToken ct = default)
        {
            return await _scheduler.CheckExists(new JobKey(taskId.ToString()), ct);
        }

        /// <summary>
        /// Получить все запланированные ID задач
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<IReadOnlyList<TId>> GetAllScheduledJobIdsAsync(CancellationToken ct = default)
        {
            var jobKeys = await _scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup(), ct);
            var ids = new List<TId>();

            foreach (var key in jobKeys)
            {
                try
                {
                    var id = ParseId(key.Name);
                    ids.Add(id);
                }
                catch // Пропускаем ключи, которые нельзя распарсить
                {
                    
                }
            }

            return ids;
        }

        /// <summary>
        /// Получить информацию о задаче в планировщике
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<IJobDetail?> GetJobDetailAsync(TId taskId, CancellationToken ct = default)
        {
            return await _scheduler.GetJobDetail(new JobKey(taskId.ToString()), ct);
        }

        /// <summary>
        /// Получить информацию о триггере задачи
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<ITrigger?> GetTriggerAsync(TId taskId, CancellationToken ct = default)
        {
            return await _scheduler.GetTrigger(new TriggerKey($"{taskId}-trigger"), ct);
        }

        #region Приватные методы

        private TId ParseId(string idStr)
        {
            if (string.IsNullOrEmpty(idStr))
                throw new InvalidOperationException("Job key cannot be null or empty");

            try
            {
                if (typeof(TId) == typeof(Guid))
                {
                    if (Guid.TryParse(idStr, out var guid))
                        return (TId)(object)guid;

                    throw new InvalidOperationException($"Invalid Guid format: {idStr}");
                }

                if (typeof(TId) == typeof(long))
                {
                    if (long.TryParse(idStr, out var longId))
                        return (TId)(object)longId;

                    throw new InvalidOperationException($"Invalid Int64 format: {idStr}");
                }

                if (typeof(TId) == typeof(int))
                {
                    if (int.TryParse(idStr, out var intId))
                        return (TId)(object)intId;

                    throw new InvalidOperationException($"Invalid Int32 format: {idStr}");
                }

                if (typeof(TId) == typeof(string))
                    return (TId)(object)idStr;

                throw new NotSupportedException($"Unsupported ID type: {typeof(TId)}");
            }
            catch (Exception ex) when (ex is not InvalidOperationException and not NotSupportedException)
            {
                throw new InvalidOperationException($"Failed to parse JobKey '{idStr}' to type {typeof(TId)}", ex);
            }
        }

        #endregion
    }
}