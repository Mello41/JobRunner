using System.ComponentModel.DataAnnotations;

namespace JobRunner.Core.Models.Enums
{
    /// <summary>
    /// Режим выполнения (определение компьютера, где будет 
    /// выполняться задача как target system)
    /// </summary>
    public enum ExecutionMode
    {
        [Display(Name = "На сервере с JobRunner")]
        Server,

        [Display(Name = "на конкретном узле")]
        SpecificNode,  

        [Display(Name = "на любом доступном")]
        AnyNode,      

        [Display(Name = "по очереди")]
        RoundRobin    
    }
}
