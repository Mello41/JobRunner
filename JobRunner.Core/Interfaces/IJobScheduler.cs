using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobRunner.Core.Interfaces
{
    /// <summary>
    /// Сборник методов по задаче для реализации в UI
    /// </summary>
    public interface IJobScheduler
    {
        #region Планировщик
        Task StartProgramAsync();

        Task StopProgramAsync();
        #endregion

        #region Задача
        Task<string> CreateTaskAsync(JobTask task);

        Task<JobTask?> GetTaskByIdAsync(long id);
        Task<JobTask?> GetTaskByPIDAsync(long pid);

        Task<bool> DeleteTaskAsync(long taskid);

        /// <summary>
        /// Остановка (пауза) задачи
        /// </summary>
        /// <param name="taskid"></param>
        /// <returns></returns>
        Task<bool> StopTaskAsync(long taskid);

        Task<bool> UpdateTaskAsync(JobTask task);

        Task<IReadOnlyList<JobTask>> GetAllTasksList();

        Task<bool> TriggerNowAsync(long taskId);
        Task<bool> ResumeTaskAsync(long taskId);
        #endregion
    }
}
