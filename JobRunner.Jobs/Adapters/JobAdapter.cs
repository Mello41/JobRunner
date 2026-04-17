using JobRunner.Core;
using JobRunner.Core.Events;
using JobRunner.Core.Interfaces;
using Quartz;
using System.Diagnostics;

namespace JobRunner.Jobs.Adapters
{
    /// <summary>
    /// Логика выполнения задачи
    /// </summary>
    public class JobAdapter : IJob
    {
        public event EventHandler<JobExecutionEventArgs>? JobExecuted;

        private void OnJobExecuted(JobExecutionEventArgs args)
        {
            JobExecuted?.Invoke(this, args);
        }

        private readonly IStringEncryptor _stringEncryptor;

        private readonly Dictionary<long, JobTask> _tasksDict;

        public JobAdapter(IStringEncryptor stringEncryptor, Dictionary<long, JobTask> tasks)
        {
            _stringEncryptor = stringEncryptor;
            _tasksDict = tasks;
        }

        /// <summary>
        /// Выполнение полного цикла запуска задачи
        /// </summary>
        /// <param name="context"> контекст выполнения задачи в Quartz 
        /// как паспорт </param>
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
            string arguments = GetTaskArguments(task);

            // 4. Запуск и ожидание (через Process)
            await StartTaskProcess(task, arguments);

            // 6. JobExecuted() для UI
            NotifyUI(task);
        }

        /// <summary>
        /// Получить строку с аргументами из такси
        /// </summary>
        /// <param name="task">приходящая таска</param>
        /// <returns></returns>
        private string GetTaskArguments(JobTask task)
        {
            string args = string.Empty;
            if (task.IsEncrypt)
            {
                if (string.IsNullOrEmpty(task.EncryptedArguments))
                    args = string.Empty;
                else
                    args = _stringEncryptor.Decrypt(task.EncryptedArguments);
            }
            else
                args = task.Arguments ?? string.Empty;

            return args;
        }

        /// <summary>
        /// Запуск процесса таски через Process
        /// </summary>
        /// <param name="task">приходящая таска</param>
        /// <param name="arguments">аргументы (получили ранее)</param>
        /// <returns></returns>
        private async Task StartTaskProcess(JobTask task, string arguments)
        {
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
        }

        /// <summary>
        /// Автоматическое уведомление UI о том, что таска создалась
        /// </summary>
        /// <param name="task">приходящая таска</param>
        /// <returns></returns>
        private void NotifyUI(JobTask task)
        {
            OnJobExecuted(new JobExecutionEventArgs
            {
                TaskId = task.Id,
                TaskName = task.Name,
                Success = string.IsNullOrEmpty(task.LastError),
                ErrorMessage = task.LastError,
                ExecutionTime = DateTime.Now,
                Duration = task.EndRun - task.StartRun
            });
        }
    }
}
