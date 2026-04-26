using JobRunner.Core.DefaultImplementations;
using JobRunner.Core.Entities;
using JobRunner.Core.Interfaces.Core;
using JobRunner.Core.Interfaces.EntityServices;
using Quartz;

namespace JobRunner.Quartz.Adapters
{
    /// <summary>
    /// Адаптер для выполнения задачи через Quartz
    /// </summary>
    public class JobAdapter : IJob
    {
        private readonly ITaskService<JobTask> _storage;
        private readonly IEncryptionService _encryption; 

        public JobAdapter(ITaskService<JobTask> storage, 
            IEncryptionService encryption)
        {
            _storage = storage;
            _encryption = encryption; 
        }

        /// <summary>
        /// Выполнение задачи при срабатывании триггера Quartz
        /// </summary>
        /// <param name="context">это контекст выполнения задачи в Quartz</param>
        /// <returns></returns>
        /// <remarks>
        /// Quartz передаёт его в метод Execute, 
        /// чтобы ты можно было получить информацию о текущем запуске
        /// </remarks>
        public async Task Execute(IJobExecutionContext context)
        {
            var taskIdStr = context.MergedJobDataMap.GetString("TaskId");
            var taskId = Guid.Parse(taskIdStr);

            // TODO: Получить задачу из ITaskStorage и
                // выполнить (уже на сервере реализация)

            await Task.CompletedTask;
        }
    }
}
