using System;
using System.ComponentModel;

namespace JobRunner.Core.Entities.ValueObjects.Settings
{
    /// <summary>
    /// Расписание для однократного 
    /// выполнения задачи в указанное время
    /// </summary>
    public sealed record OnceSchedule : IScheduleSettings
    {
        /// <summary>
        /// Дата и время однократного запуска
        /// </summary>
        [DisplayName("Время запуска")]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Возвращает человекочитаемое описание расписания
        /// </summary>
        /// <returns>
        public string GetDescription() =>
            $"Однократно {StartTime:dd.MM.yyyy HH:mm:ss}";

        /// <summary>
        /// Проверяет, корректны ли параметры расписания
        /// </summary>
        /// <returns></returns>
        public bool IsValid() => StartTime != default;

        /// <summary>
        /// Проверяет, нужно ли выполнять задачу
        /// </summary>
        /// <param name="isCompleted">Флаг из IJobTaskMetadata.IsCompleted</param>
        public bool ShouldExecute(bool isCompleted)
        {
            if (isCompleted) return false;
            return StartTime <= DateTime.Now;
        }
    }
}