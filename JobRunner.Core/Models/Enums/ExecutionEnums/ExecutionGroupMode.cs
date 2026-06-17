using System.ComponentModel.DataAnnotations;

namespace JobRunner.Core.Models.Enums.ExecutionEnums
{
    /// <summary>
    /// Режим выполнения задач в группе
    /// </summary>
    public enum ExecutionGroupMode
    {
        /// <summary>
        /// Параллельное выполнение (все задачи запускаются одновременно)
        /// </summary>
        [Display(Name = "Параллельно")]
        Parallel = 0,

        /// <summary>
        /// Последовательное выполнение (одна задача за другой)
        /// </summary>
        [Display(Name = "Последовательно")]
        Sequential = 1,

        /// <summary>
        /// Конвейер (результат предыдущей задачи передаётся следующей)
        /// </summary>
        [Display(Name = "Конвейер")]
        Pipeline = 2,

        /// <summary>
        /// По одному узлу (задачи распределяются по разным узлам)
        /// </summary>
        [Display(Name = "По одному узлу")]
        RoundRobin = 3
    }
}
