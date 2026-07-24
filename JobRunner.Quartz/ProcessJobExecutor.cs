using JobRunner.Core.DTO.Results;
using JobRunner.Core.Events.TaskEvents.TaskStatus;
using JobRunner.Core.Interfaces.Entities;
using JobRunner.Core.Interfaces.Execution;
using JobRunner.Core.Utils.Platform;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text;

namespace JobRunner.Quartz
{
    /// <summary>
    /// Кросс-платформенный исполнитель задач, запускающий внешние процессы.
    /// Поддерживает Windows и Linux, автоматически определяет операционную систему
    /// и применяет соответствующую стратегию запуска и остановки процессов.
    /// </summary>
    /// <typeparam name="TId">Тип идентификатора задачи</typeparam>
    public class ProcessJobExecutor<TId> : IJobExecutor<TId> where TId : IEquatable<TId>
    {
        private readonly ILogger<ProcessJobExecutor<TId>> _logger;

        public event Func<TaskStartedEvent<TId>, Task>? TaskStarted;
        public event Func<TaskCompletedEvent<TId>, Task>? TaskCompleted;

        /// <summary>
        /// ctor
        /// Инициализирует новый экземпляр исполнителя процессов.
        /// </summary>
        /// <param name="logger">Логгер для записи диагностической информации</param>
        public ProcessJobExecutor(ILogger<ProcessJobExecutor<TId>> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Выполняет задачу, запуская внешний 
        /// процесс с учетом особенностей ОС
        /// </summary>
        /// <param name="task"></param>
        /// <param name="ct"></param>
        /// <returns>Результат выполнения с данными о процессе и выводе</returns>
        public async Task<JobExecutionResult> ExecuteAsync(IJobTask<TId> task, CancellationToken ct = default)
        {
            var startTime = DateTime.UtcNow;
            long? processId = null;

            try
            {
                var (fileName, arguments) = NormalizeForPlatform(task);

                using var process = new Process();

                if (PlatformDetector.IsWindows)
                {
                    ConfigureWindowsProcess(process, fileName, arguments);
                }
                else if (PlatformDetector.IsLinux)
                {
                    ConfigureLinuxProcess(process, fileName, arguments);
                }
                else
                {
                    throw new PlatformNotSupportedException($"Platform not supported");
                }

                process.Start();
                processId = process.Id;

                await OnTaskStarted(task, processId, startTime, ct);

                // Синхронный режим (ждем завершения)
                if (!task.IsAsyncExecution)
                {
                    var outputTask = ReadStreamAsync(process.StandardOutput, ct);
                    var errorTask = ReadStreamAsync(process.StandardError, ct);

                    await WaitForExitAsync(process, task.TimeoutSeconds, ct);

                    var output = await outputTask;
                    var error = await errorTask;

                    var result = CreateResult(process, startTime, processId, output, error);
                    await OnTaskCompleted(task, result, ct);
                    return result;
                }

                // Асинхронный режим (не ждем завершения)
                _logger.LogDebug("Async execution mode: process {ProcessId} started", processId);
                var asyncResult = JobExecutionResult.CreateSuccess(processId, startTime);
                await OnTaskCompleted(task, asyncResult, ct);
                return asyncResult;
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                _logger.LogWarning("Task {TaskId} was cancelled", task.Id);
                await KillProcessTreeAsync(processId, ct);
                var cancelledResult = JobExecutionResult.CreateFailure("Cancelled", startTime);
                await OnTaskCompleted(task, cancelledResult, ct);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Task {TaskId} failed: {ErrorMessage}", task.Id, ex.Message);
                var errorResult = JobExecutionResult.CreateFailure(ex.Message, startTime);
                await OnTaskCompleted(task, errorResult, ct);
                throw;
            }
        }

        #region приватные методы
        /// <summary>
        /// Нормализует путь и аргументы в зависимости от платформы.
        /// </summary>
        /// <param name="task">Задача для нормализации</param>
        /// <returns>Кортеж из нормализованного пути и аргументов</returns>
        private (string FileName, string Arguments) NormalizeForPlatform(IJobTask<TId> task)
        {
            var path = task.ExecutionPath;
            var args = task.ScheduleArguments?.BuildCommandLineArguments() ?? "";
            var escaper = PlatformDetector.CreateEscaper();

            if (PlatformDetector.IsWindows)
            {
                var ext = Path.GetExtension(path).ToLower();
                if (ext == ".bat" || ext == ".cmd")
                    return ("cmd.exe", $"/c \"{escaper.EscapePath(path)}\" {args}");
                return (path, args);
            }

            if (PlatformDetector.IsLinux)
            {
                if (!IsExecutable(path)) // проверка на исполняемость файла
                    TryMakeExecutable(path);
                return (path, args);
            }

            return (path, args);
        }

        /// <summary>
        /// Ожидание завершения процесса с поддержкой таймаута
        /// </summary>
        /// <param name="process"></param>
        /// <param name="timeoutSeconds">таймаут (секунды) (берется из задачи)</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        /// <exception cref="TimeoutException">При превышении таймаута</exception>
        private static async Task WaitForExitAsync(Process process, int? timeoutSeconds, CancellationToken ct)
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            if (timeoutSeconds.HasValue && timeoutSeconds.Value > 0)
                cts.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds.Value));

            try
            {
                await process.WaitForExitAsync(cts.Token);
            }
            catch (OperationCanceledException) when (cts.IsCancellationRequested && !ct.IsCancellationRequested)
            {
                if (PlatformDetector.IsWindows)
                    process.Kill(true);
                else
                    await KillProcessTreeAsync(process.Id, CancellationToken.None);

                // выброс Exception
                throw new TimeoutException($"Task exceeded timeout of {timeoutSeconds} seconds");
            }
        }

        /// <summary>
        /// Принудительно завершает процесс и все его дерево
        /// </summary>
        /// <param name="pid">Идентификатор процесса (PID) 
            /// (существует только когда задача исполняется)</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>true если процесс был завершен, false в противном случае</returns>
        private static async Task<bool> KillProcessTreeAsync(long? pid, CancellationToken ct)
        {
            if (!pid.HasValue) return false;

            if (PlatformDetector.IsWindows)
            {
                try
                {
                    var process = Process.GetProcessById((int)pid.Value);
                    process.Kill(true);
                    await process.WaitForExitAsync(ct);
                    return true;
                }
                catch (ArgumentException)
                {
                    return false;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to kill process {pid}: {ex.Message}");
                    return false;
                }
            }

            if (PlatformDetector.IsLinux)
            {
                try
                {
                    await RunBashCommand($"pkill -TERM -P {pid} && kill -TERM {pid}");

                    var maxWaitMs = 10000; 
                    var waitStep = 200;    
                    var elapsed = 0;

                    while (elapsed < maxWaitMs && !ct.IsCancellationRequested)
                    {
                        await Task.Delay(waitStep, ct);
                        elapsed += waitStep;

                        if (!await IsProcessRunning(pid.Value))
                        {
                            return true;
                        }
                    }

                    if (await IsProcessRunning(pid.Value))
                    {
                        await RunBashCommand($"pkill -KILL -P {pid} && kill -KILL {pid}");
                        await Task.Delay(500, ct);
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to kill process tree {pid}: {ex.Message}");
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Создает результат выполнения из завершенного процесса
        /// свойства берутся из класса JobExecutionResult
        /// </summary>
        /// <param name="process"></param>
        /// <param name="startTime"></param>
        /// <param name="processId"></param>
        /// <param name="standardOutput"></param>
        /// <param name="standardError"></param>
        /// <returns></returns>
        private static JobExecutionResult CreateResult(Process process, 
                                DateTime startTime, long? processId,
                                string standardOutput, string standardError)
        {
            var endTime = DateTime.UtcNow;
            return new JobExecutionResult
            {
                Success = process.ExitCode == 0,
                ProcessId = processId,
                ExitCode = process.ExitCode,
                StartTime = startTime,
                EndTime = endTime,
                DurationMs = (long)(endTime - startTime).TotalMilliseconds,
                StandardOutput = standardOutput,
                StandardError = standardError
            };
        }

        #region по событиям
        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <param name="processId"></param>
        /// <param name="startTime"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        private async Task OnTaskStarted(IJobTask<TId> task, long? processId, DateTime startTime, CancellationToken ct)
        {
            if (TaskStarted != null)
            {
                var evt = new TaskStartedEvent<TId>
                {
                    TaskId = task.Id,
                    TaskName = task.Name,
                    StartTime = startTime,
                    ProcessId = processId,
                    ExecutionPath = task.ExecutionPath
                };
                await TaskStarted.Invoke(evt);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <param name="result"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        private async Task OnTaskCompleted(IJobTask<TId> task, JobExecutionResult result, CancellationToken ct)
        {
            if (TaskCompleted != null)
            {
                var evt = new TaskCompletedEvent<TId>
                {
                    TaskId = task.Id,
                    TaskName = task.Name,
                    Success = result.Success,
                    ErrorMessage = result.ErrorMessage,
                    CompletionTime = DateTime.UtcNow,
                    DurationMs = result.DurationMs
                };
                await TaskCompleted.Invoke(evt);
            }
        }
        #endregion

        #region проверки файла
        private static bool IsExecutable(string path)
        {
            if (!File.Exists(path)) return false;
            try
            {
                var fileInfo = new FileInfo(path);
                return (fileInfo.UnixFileMode & UnixFileMode.UserExecute) != 0;
            }
            catch
            {
                return false;
            }
        }

        private static void TryMakeExecutable(string path)
        {
            try
            {
                var chmod = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "chmod",
                        Arguments = $"+x \"{path}\"",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                chmod.Start();
                chmod.WaitForExit();
            }
            catch { }
        }

        /// <summary>
        /// Проверяет, запущен ли процесс с указанным PID
        /// </summary>
        /// <param name="pid">PID процесса (метаданные задачи)</param>
        /// <returns></returns>
        private static async Task<bool> IsProcessRunning(long pid)
        {
            try
            {
                var process = Process.GetProcessById((int)pid);
                return !process.HasExited;
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }
        #endregion

        /// <summary>
        /// Выполняет bash команду и возвращает вывод
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private static async Task<string> RunBashCommand(string command)
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{command}\"",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                }
            };

            process.Start();
            var output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();
            return output;
        }

        /// <summary>
        /// 
        /// если этого не сделать, то:
        /// await process.WaitForExitAsync(); <-- может зависнуть навсегда!
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        private static async Task<string> ReadStreamAsync(StreamReader reader, CancellationToken ct)
        {
            var sb = new StringBuilder();
            try
            {
                while (!reader.EndOfStream && !ct.IsCancellationRequested)
                {
                    var line = await reader.ReadLineAsync(ct);
                    if (line != null) sb.AppendLine(line);
                }
            }
            catch (OperationCanceledException)
            {
                // Ожидаемая отмена
            }
            return sb.ToString();
        }

        #region Настройка под ОС (пока что Windows & Linux)
        /// <summary>
        /// Настройка процесса под Windows
        /// </summary>
        /// <param name="process"></param>
        /// <param name="fileName"></param>
        /// <param name="arguments"></param>
        private static void ConfigureWindowsProcess(Process process, string fileName, 
                                                    string arguments)
        {
            process.StartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };
        }

        /// <summary>
        /// Настройка процесса под Linux
        /// </summary>
        /// <param name="process"></param>
        /// <param name="fileName"></param>
        /// <param name="arguments"></param>
        private static void ConfigureLinuxProcess(Process process, string fileName, string arguments)
        {
            var escaper = PlatformDetector.CreateEscaper();
            var safeFileName = escaper.EscapeArgument(fileName);
            var safeArgs = escaper.EscapeArgument(arguments);

            process.StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c '{safeFileName} {safeArgs}'", 
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };
        }
        #endregion

        #endregion
    }
}