using System.ComponentModel.DataAnnotations;

namespace JobRunner.Core.Models.Enums
{
    /// <summary>
    /// Состояние сущности в Unit of Work
    /// </summary>
    public enum EntityState
    {
        [Display(Name = "Сущность не отслеживается")]
        Detached = 0,

        [Display(Name = "Сущность новая, будет добавлена")]
        Added = 1,

        [Display(Name = "Сущность изменена, будет обновлена")]
        Modified = 2,

        [Display(Name = "Сущность удалена")]
        Deleted = 3,

        [Display(Name = "Сущность не изменилась")]
        Unchanged = 4
    }
}
