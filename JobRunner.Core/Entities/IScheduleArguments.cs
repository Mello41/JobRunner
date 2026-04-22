using JobRunner.Core.DTO;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JobRunner.Core.Entities
{
    /// <summary>
    /// Интерфейс расширяемого набора аргументов задачи
    /// </summary>
    public interface IScheduleArguments
    {
        /// <summary>
        /// Список аргументов задачи
        /// </summary>
        [Display(Name = "Аргументы задачи JobTask", Description = "Список аргументов командной строки")]
        List<ScheduleArgumentItem> Items { get; set; }
    }
}
