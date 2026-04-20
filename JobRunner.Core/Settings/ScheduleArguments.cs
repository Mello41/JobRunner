using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JobRunner.Core.Settings
{
    /// <summary>
    /// Расширяемый набор аргументов задачи
    /// </summary>
    public class ScheduleArguments
    {
        /// <summary>
        /// Список аргументов задачи
        /// </summary>
        [Display(Name = "Аргументы задачи JobTask", Description = "Список аргументов командной строки")]
        public List<ScheduleArgumentItem> Items { get; set; } = new List<ScheduleArgumentItem>();
    }
}
