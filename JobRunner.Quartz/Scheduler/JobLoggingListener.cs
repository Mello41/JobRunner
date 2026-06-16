using JobRunner.Core.Interfaces.Scheduler;
using Microsoft.Extensions.Logging;
using Quartz;

namespace JobRunner.Quartz.Scheduler
{
    /// <summary>
    /// Слушатель для логирования всех событий выполнения задач в Quartz
    /// </summary>
    /// <remarks>
    /// Реализует IJobListener из Quartz.NET и IJobExecutionListener из Core
    /// </remarks>
    public class JobLoggingListener : IJobListener, IJobExecutionListener
    {
        private readonly ILogger<JobLoggingListener> _logger;

        public string Name => "JobLoggingListener";

        public JobLoggingListener(ILogger<JobLoggingListener> logger)
        {
            _logger = logger;
        }

        public async Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            var jobKey = context.JobDetail.Key;
            var jobData = context.MergedJobDataMap;

            _logger.LogInformation(
                "📋 Job {JobKey} is about to be executed. Trigger: {TriggerKey}",
                jobKey, context.Trigger.Key);

            await OnJobExecutingAsync(jobKey, jobData, cancellationToken);
        }

        public async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException? jobException, CancellationToken cancellationToken = default)
        {
            var jobKey = context.JobDetail.Key;
            var jobData = context.MergedJobDataMap;

            if (jobException != null)
            {
                _logger.LogError(jobException,
                    "❌ Job {JobKey} failed with exception: {ErrorMessage}",
                    jobKey, jobException.Message);

                await OnJobExecutedAsync(jobKey, jobData, false, jobException.Message, cancellationToken);
            }
            else
            {
                _logger.LogInformation(
                    "✅ Job {JobKey} completed successfully. Duration: {Duration}",
                    jobKey, context.JobRunTime);

                await OnJobExecutedAsync(jobKey, jobData, true, null, cancellationToken);
            }
        }

        public async Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            var jobKey = context.JobDetail.Key;
            var jobData = context.MergedJobDataMap;

            _logger.LogWarning(
                "⛔ Job {JobKey} execution was vetoed. Trigger: {TriggerKey}",
                jobKey, context.Trigger.Key);

            await OnJobExecutionVetoedAsync(jobKey, jobData, "Vetoed by trigger or listener", cancellationToken);
        }

        #region Реализация IJobExecutionListener

        public Task OnJobExecutingAsync(object jobKey, object? jobData, CancellationToken ct = default)
        {
            // Можно добавить дополнительную логику, например:
            // - Отправить метрику в Prometheus
            // - Записать в Audit лог
            // - Обновить статус в UI (через SignalR)
            return Task.CompletedTask;
        }

        public Task OnJobExecutedAsync(object jobKey, object? jobData, bool success, string? errorMessage = null, CancellationToken ct = default)
        {
            // Можно добавить дополнительную логику
            return Task.CompletedTask;
        }

        public Task OnJobExecutionVetoedAsync(object jobKey, object? jobData, string? reason = null, CancellationToken ct = default)
        {
            // Можно добавить дополнительную логику
            return Task.CompletedTask;
        }

        #endregion
    }
}
