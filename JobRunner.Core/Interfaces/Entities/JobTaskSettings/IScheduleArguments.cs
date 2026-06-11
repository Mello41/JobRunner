using JobRunner.Core.DTO;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JobRunner.Core.Interfaces.Entities.JobTaskSettings
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

        /// <summary>
        /// Построить безопасную командную строку с экранированием
        /// </summary>
        string BuildCommandLineArguments();
    }
}
