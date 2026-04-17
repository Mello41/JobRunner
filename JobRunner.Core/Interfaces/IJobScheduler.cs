using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobRunner.Core.Interfaces
{
    /// <summary>
    /// 
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
        Task<bool> StopTaskAsync(long taskid);

        Task<bool> UpdateTaskAsync(JobTask task);

        Task<IReadOnlyList<JobTask>> GetAllTasksList();
        #endregion
    }
}
