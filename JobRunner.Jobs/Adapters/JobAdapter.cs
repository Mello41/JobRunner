using JobRunner.Core;
using JobRunner.Core.Interfaces;
using Quartz;
using System.Diagnostics;

namespace JobRunner.Jobs.Adapters
{
    /// <summary>
    /// Логика выполнения 
    /// </summary>
    public class JobAdapter : IJob
    {
        /* UI 
        public event EventHandler<JobExecutionEventArgs>? JobExecuted;

        private void OnJobExecuted(JobExecutionEventArgs args)
        {
            JobExecuted?.Invoke(this, args);
        }*/

        private readonly IStringEncryptor _stringEncryptor;

        private readonly Dictionary<long, JobTask> _tasksDict;

        public JobAdapter(IStringEncryptor stringEncryptor, Dictionary<long, JobTask> tasks)
        {
            _stringEncryptor = stringEncryptor;
            _tasksDict = tasks;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Execute(IJobExecutionContext context)
        {
            // 1. получить ID задачи
            string taskIdStr = context.MergedJobDataMap.GetString("TaskId");
            long taskId = long.Parse(taskIdStr);

            // 2. Найти задачу по ID
            if (!_tasksDict.TryGetValue(taskId, out var task))
                return; // ContainsKey медленнее

            // 3. Если зашифровано - расшифровать аргументы
            string arguments = string.Empty;
            if (task.IsEncrypt)
            {
                if (string.IsNullOrEmpty(task.EncryptedArguments))
                    arguments = string.Empty;
                else
                    arguments = _stringEncryptor.Decrypt(task.EncryptedArguments);
            }
            else
                arguments = task.Arguments ?? string.Empty;

            // 4. Запуск и ожидание (через Process)
            try
            {
                var process = new Process();
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = task.ExecutionPath,
                    Arguments = arguments,

                    UseShellExecute = false,
                    CreateNoWindow = true,

                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                process.Start();

                task.PID = process.Id;
                task.StartRun = DateTime.Now;
                task.IsRunning = true;
                task.IsEnabled = true;

                // 5. Запись результатов (+ случай ошибки)
                if (!task.IsAsyncExecution)
                {
                    await process.WaitForExitAsync();

                    task.EndRun = DateTime.Now;
                    task.IsRunning = false;
                    task.IsCompleted = process.ExitCode == 0;
                    task.LastRun = DateTime.Now;
                    task.LastError = process.ExitCode != 0 ? $"Exit code: {process.ExitCode}" : "";
                }
            }
            catch (Exception ex)
            {
                task.LastError = ex.Message;
                task.IsCompleted = false;
                task.IsRunning = false;
            }

            // 6. JobExecuted() для UI

        }
    }
}
