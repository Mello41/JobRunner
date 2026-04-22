using JobRunner.Core.DTO;
using JobRunner.Core.Entities;
using System.Threading.Tasks;

namespace JobRunner.Core.Interfaces
{
    /// <summary>
    /// Сервис для шифрования чувствительных данных
    /// </summary>
    public interface IEncryptionService
    {
        /// <summary>
        /// Зашифровать значение аргумента
        /// </summary>
        string EncryptArgument(ScheduleArgumentItem argument);

        /// <summary>
        /// Расшифровать значение аргумента
        /// </summary>
        string DecryptArgument(ScheduleArgumentItem argument);

        /// <summary>
        /// Зашифровать все чувствительные аргументы задачи
        /// </summary>
        Task EncryptSensitiveArgumentsAsync(IScheduleArguments arguments);

        /// <summary>
        /// Расшифровать все чувствительные аргументы задачи
        /// </summary>
        Task DecryptSensitiveArgumentsAsync(IScheduleArguments arguments);
    }
}
