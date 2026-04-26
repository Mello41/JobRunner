namespace JobRunner.Core.Entities.ValueObjects.Settings
{
    /// <summary>
    /// Расписание на основе Cron-выражения
    /// </summary>
    public sealed record CronSchedule : IScheduleSettings
    {
        /// <summary>
        /// Cron-выражение (формат Quartz)
        /// </summary>
        public string CronExpression { get; set; } = string.Empty;

        public string ToCronExpression() => CronExpression;

        public string GetDescription() => $"Cron: {CronExpression}";

        public bool IsValid() => !string.IsNullOrEmpty(CronExpression);
    }
}
