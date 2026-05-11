using JobRunner.Core.Entities;
using JobRunner.Core.Events.TaskEvents.TaskStatus;
using JobRunner.Core.Results;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JobRunner.Core.Interfaces.Execution
{
    /// <summary>
    /// Исполнитель задачи — отвечает за низкоуровневый запуск внешнего процесса.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Назначение:</b>
    /// Этот интерфейс является технической абстракцией для запуска внешних исполняемых файлов.
    /// Он НЕ занимается бизнес-логикой (обновление БД, события, уведомления) — только запуск процесса.
    /// </para>
    /// 
    /// <para>
    /// <b>Ответственности:</b>
    /// <list type="bullet">
    ///   <item>Запуск процесса по указанному пути <see cref="IJobTask.ExecutionPath"/></item>
    ///   <item>Передача аргументов командной строки из <see cref="IJobTask.ScheduleArguments"/></item>
    ///   <item>Применение таймаута <see cref="IJobTask.TimeoutSeconds"/> (если задан)</item>
    ///   <item>Учёт флага <see cref="IJobTask.IsAsyncExecution"/> (ожидать завершения или нет)</item>
    ///   <item>Сбор stdout/stderr процесса</item>
    ///   <item>Возврат результата в виде <see cref="JobExecutionResult"/></item>
    /// </list>
    /// </para>
    /// 
    /// <para>
    /// <b>Что НЕ входит в ответственность исполнителя:</b>
    /// <list type="bullet">
    ///   <item>Обновление метаданных задачи (<see cref="IJobTaskMetadata"/>)</item>
    ///   <item>Публикация доменных событий (<see cref="IDomainEventDispatcher"/>)</item>
    ///   <item>Работа с базой данных</item>
    ///   <item>Отправка уведомлений</item>
    ///   <item>Логирование (кроме технических ошибок)</item>
    /// </list>
    /// </para>
    /// 
    /// <para>
    /// <b>Где находится реализация:</b>
    /// Реализация этого интерфейса должна находиться в слое Infrastructure (например, JobRunner.Infrastructure),
    /// так как она зависит от System.Diagnostics.Process и платформозависимых API.
    /// </para>
    /// 
    /// <para>
    /// <b>Пример использования:</b>
    /// <code>
    /// var result = await jobExecutor.ExecuteAsync(task, cancellationToken);
    /// if (result.Success)
    /// {
    ///     // Обработка успешного выполнения
    /// }
    /// else if (result.IsTimeout)
    /// {
    ///     // Обработка таймаута
    /// }
    /// </code>
    /// </para>
    /// </remarks>
    public interface IJobExecutor
    {
        event Func<TaskStartedEvent, Task>? TaskStarted;
        event Func<TaskCompletedEvent, Task>? TaskCompleted;

        /// <summary>
        /// Выполнить задачу
        /// </summary>
        /// <param name="task">Задача для выполнения</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат выполнения</returns>
        /// <remarks>
        /// Если у задачи задан TimeoutSeconds, должен использоваться linked token source
        /// с таймаутом. При превышении таймаута процесс принудительно завершается.
        /// </remarks>
        Task<JobExecutionResult> ExecuteAsync(IJobTask task, CancellationToken ct = default);
    }
}
