using Quartz;

namespace JobRunner.Quartz.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class CronValidationExtensions
    {
        /// <summary>
        /// Проверяет, является ли строка корректным Cron-выражением
        /// </summary>
        /// <param name="cronExpression"></param>
        /// <returns></returns>
        public static bool IsValidCron(this string cronExpression)
        {
            if (string.IsNullOrWhiteSpace(cronExpression))
                return false;

            try
            {
                return CronExpression.IsValidExpression(cronExpression);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Проверяет Cron-выражение и выбрасывает исключение при ошибке
        /// </summary>
        /// <param name="cronExpression"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void ValidateCron(this string cronExpression)
        {
            if (string.IsNullOrWhiteSpace(cronExpression))
                throw new ArgumentException("Cron expression cannot be null or empty", nameof(cronExpression));

            if (!CronExpression.IsValidExpression(cronExpression))
                throw new ArgumentException($"Invalid cron expression: {cronExpression}", nameof(cronExpression));
        }
    }
}
