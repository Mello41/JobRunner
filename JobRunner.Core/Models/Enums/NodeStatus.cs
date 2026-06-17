using System.ComponentModel.DataAnnotations;

namespace JobRunner.Core.Models.Enums
{
    /// <summary>
    /// Статус узла (ноды) в распределенной системе выполнения задач
    /// </summary>
    public enum NodeStatus
    {
        [Display(Name = "Офлайн (выключено)")]
        Online,

        [Display(Name = "Онлайн (включено)")]
        Offline,

        [Display(Name = "Занято")]
        Busy,

        [Display(Name = "На обслуживании")]
        Maintenance
    }
}