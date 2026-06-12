using JobRunner.Core.DTO;
using JobRunner.Core.Interfaces.Entities.JobTaskSettings;
using JobRunner.Core.Interfaces.Platform.CommandLine;
using JobRunner.Core.Utils.Platform;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace JobRunner.Core.DefaultImplementations
{
    /// <summary>
    /// Расширяемый набор аргументов задачи
    /// </summary>
    public class ScheduleArgumentsExample : IScheduleArguments
    {
        /// <summary>
        /// Список аргументов задачи
        /// </summary>
        [Display(Name = "Аргументы задачи JobTask", Description = "Список аргументов командной строки")]
        public List<ScheduleArgumentItem> Items { get; set; } = new List<ScheduleArgumentItem>();

        private readonly ICommandLineEscaper _escaper;

        public ScheduleArgumentsExample()
        {
            _escaper = PlatformDetector.CreateEscaper();
        }

        public string BuildCommandLineArguments()
        {
            if (Items == null || Items.Count == 0)
                return string.Empty;

            return string.Join(" ", Items.Select(item =>
                $"{_escaper.EscapeArgument(item.Key)} {_escaper.EscapeArgument(item.Value?.ToString() ?? string.Empty)}"));
        }
    }
}
