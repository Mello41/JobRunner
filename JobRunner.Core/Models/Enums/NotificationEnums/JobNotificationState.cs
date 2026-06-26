using System.ComponentModel.DataAnnotations;

namespace JobRunner.Core.Models.Enums.NotificationEnums
{
    /// <summary>
    /// Типы состояний задачи для уведомлений
    /// </summary>
    public enum JobNotificationState
    {
        #region До
        [Display(Name = "Перед созданием")]
        BeforeCreated = -100,

        [Display(Name = "Перед запуском")]
        BeforeStarted = -99,

        [Display(Name = "Перед завершением")]
        BeforeCompleted = -98,

        [Display(Name = "Перед остановкой")]
        BeforeStopped = -97,

        [Display(Name = "Перед паузой")]
        BeforePaused = -96,

        [Display(Name = "Перед возобновлением")]
        BeforeResumed = -95,

        [Display(Name = "Перед пропуском")]
        BeforeSkipped = -94,
        #endregion

        #region После 
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
        #endregion
    }
}