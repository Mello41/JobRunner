using System.ComponentModel.DataAnnotations;

namespace JobRunner.Core.DTO
{
    /// <summary>
    /// Отдельный аргумент задачи --> List<ScheduleArgumentItem> --> ScheduleArguments
    /// </summary>
    public class ScheduleArgumentItem
    {
        /// <summary>
        /// Ключ аргумента (например, --user, -p, --config)
        /// </summary>
        [Display(Name = "Ключ", Description = "Имя аргумента (--user, -p, --config)")]
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// Значение аргумента
        /// </summary>
        [Display(Name = "Значение", Description = "Значение аргумента")]
        public object Value { get; set; } = string.Empty;

        /// <summary>
        /// Зашифровать значение аргумента
        /// </summary>
        [Display(Name = "Шифровать", Description = "Шифровать значение аргумента")]
        public bool IsEncryptedArgument { get; set; }
    }
}
