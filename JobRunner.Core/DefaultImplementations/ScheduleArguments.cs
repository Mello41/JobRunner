using JobRunner.Core.DTO;
using JobRunner.Core.Entities.ValueObjects;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JobRunner.Core.DefaultImplementations
{
    /// <summary>
    /// Расширяемый набор аргументов задачи
    /// </summary>
    public class ScheduleArguments : IScheduleArguments
    {
        /// <summary>
        /// Список аргументов задачи
        /// </summary>
        [Display(Name = "Аргументы задачи JobTask", Description = "Список аргументов командной строки")]
        public List<ScheduleArgumentItem> Items { get; set; } = new List<ScheduleArgumentItem>();
    }
}
