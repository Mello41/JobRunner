using System.ComponentModel.DataAnnotations;

namespace JobRunner.Core.DTO.Grouping
{
    /// <summary>
    /// Изменения для применения ко всей группе задач
    /// </summary>
    public class GroupSettingsPatch
    {
        [Display(Name = "Включить/выключить все задачи группы")] 
        public bool? IsEnabled { get; set; }

        [Display(Name = "Таймаут для всех задач группы (секунды)")]
        public int? TimeoutSeconds { get; set; }

        [Display(Name = "Разрешить параллельное выполнение")]
        public bool? AllowConcurrentExecution { get; set; }
    }
}