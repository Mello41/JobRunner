namespace JobRunner.Core.Entities.ValueObjects.Settings
{
    public sealed record DailySchedule : IScheduleSettings
    {
        public int Hour { get; set; }
        public int Minute { get; set; }

        public string GetDescription() => $"Ежедневно в {Hour:D2}:{Minute:D2}";

        public bool IsValid() => Minute >= 0 && Minute <= 59 && Hour >= 0 && Hour <= 23;
    }
}
