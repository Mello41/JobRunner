using System.ComponentModel.DataAnnotations;

namespace JobRunner.Core.Entities.Enums
{
    /// <summary>
    /// Стратегии расчета задержки между повторными попытками выполнения задачи
    /// </summary>
    public enum RetryStrategyTypes
    {
        /// <summary>
        /// Без повторных попыток
        /// </summary>
        [Display(Name = "Без повторов")]
        None,

        /// <summary>
        /// Фиксированная задержка между попытками
        /// </summary>
        /// <example>InitialDelaySeconds = 5 → 5с, 5с, 5с...</example>
        [Display(Name = "Фиксированная задержка")]
        FixedDelay,

        /// <summary>
        /// Экспоненциально растущая задержка
        /// </summary>
        /// <example>
        /// InitialDelaySeconds = 2 → 2с, 4с, 8с, 16с...
        /// Задержка = InitialDelaySeconds * 2^(attemptNumber-1)
        /// </example>
        [Display(Name = "Экспоненциальная задержка")]
        ExponentialBackoff,

        /// <summary>
        /// Линейно растущая задержка
        /// </summary>
        /// <example>
        /// InitialDelaySeconds = 2 → 2с, 4с, 6с, 8с...
        /// Задержка = InitialDelaySeconds * attemptNumber
        /// </example>
        [Display(Name = "Линейная задержка")]
        Incremental
    }
}
