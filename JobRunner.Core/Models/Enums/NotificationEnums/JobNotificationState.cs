using System.ComponentModel.DataAnnotations;

namespace JobRunner.Core.Models.Enums.NotificationEnums
{
    /// <summary>
    /// Типы состояний задачи для уведомлений
    /// </summary>
    public enum JobNotificationState
    {
        [Display(Name = "При создании")]
        OnCreated = 0,

        [Display(Name = "При запуске")]
        OnStarted = 1,

        [Display(Name = "При успешном завершении")]
        OnCompleted = 2,

        [Display(Name = "При ошибке")]
        OnFailed = 3,

        [Display(Name = "При остановке")]
        OnStopped = 4,

        [Display(Name = "При паузе")]
        OnPaused = 5,

        [Display(Name = "При возобновлении")]
        OnResumed = 6,

        [Display(Name = "При пропуске")]
        OnSkipped = 7
    }
}
