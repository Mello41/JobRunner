using JobRunner.Core.DefaultImplementations;
using JobRunner.Core.Interfaces.Core;
using JobRunner.Core.Interfaces.EntityServices;
using Quartz;
using System.Diagnostics;

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
        /// (полный цикл выполнения задачи)
        /// </summary>
        /// <param name="context">это контекст выполнения задачи в Quartz</param>
        /// <returns></returns>
        public async Task Execute(IJobExecutionContext context)
        {
            var taskIdStr = context.MergedJobDataMap.GetString("TaskId");
            var taskId = Guid.Parse(taskIdStr);

            var task = await _storage.GetByIdAsync(taskId);
            if (task == null)
            {
                Console.WriteLine($"[JobAdapter] Task {taskId} not found");
                return;
            }

            // ⭐ Параллельный контроль: если запрещено, проверяем на уже запущенную задачу
            if (!task.IsAsyncExecution)
            {
                var existingProcess = Process.GetProcesses()
                    .FirstOrDefault(p => p.Id == task.JobTaskMetadata.TaskPID);
                if (existingProcess != null && !existingProcess.HasExited)
                {
                    Console.WriteLine($"[JobAdapter] Task {taskId} already running, skipping");
                    return;
                }
            }

            await _encryption.DecryptSensitiveArgumentsAsync(task.ScheduleArguments);

            var arguments = string.Join(" ", task.ScheduleArguments.Items
                .Select(a => $"{a.Key} \"{a.Value}\""));

            using var process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                FileName = task.ExecutionPath,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            try
            {
                process.Start();
                task.JobTaskMetadata.TaskPID = process.Id;
                task.StartRun = DateTime.UtcNow;
                task.JobTaskMetadata.IsRunning = true;
                task.JobTaskMetadata.LastRun = DateTime.UtcNow;

                await _storage.UpdateAsync(task);

                if (!task.IsAsyncExecution)
                {
                    await process.WaitForExitAsync();

                    task.EndRun = DateTime.UtcNow;
                    task.JobTaskMetadata.IsRunning = false;
                    task.JobTaskMetadata.IsCompleted = process.ExitCode == 0;
                    task.JobTaskMetadata.LastError = process.ExitCode != 0
                        ? await process.StandardError.ReadToEndAsync()
                        : string.Empty;
                }
                else
                {
                    task.JobTaskMetadata.LastError = string.Empty;
                }

                await _storage.UpdateAsync(task);
            }
            catch (Exception ex)
            {
                task.JobTaskMetadata.LastError = ex.Message;
                task.JobTaskMetadata.IsCompleted = false;
                task.JobTaskMetadata.IsRunning = false;
                await _storage.UpdateAsync(task);
            }
        }
    }
}
