using JobRunner.Core.DTO;
using JobRunner.Core.Entities.ValueObjects;
using JobRunner.Core.Interfaces.Core;
using System.Text;

namespace JobRunner.Domain.Tests.EncryptionTests
{
    /// <summary>
    /// Улучшенная тестовая реализация шифрования
    /// </summary>
    public class TestEncryptionService : IEncryptionService
    {
        private readonly Dictionary<string, string> _encryptionMap = new();
        private readonly object _lock = new();

        public string EncryptArgument(ScheduleArgumentItem argument)
        {
            if (argument.Value == null)
                throw new ArgumentNullException(nameof(argument.Value));

            var originalValue = argument.Value.ToString()!;

            if (string.IsNullOrEmpty(originalValue))
            {
                return "EMPTY_STRING_MARKER";
            }

            var encrypted = Convert.ToBase64String(Encoding.UTF8.GetBytes(originalValue));

            lock (_lock)
            {
                _encryptionMap[encrypted] = originalValue;
            }

            return encrypted;
        }

        public string DecryptArgument(ScheduleArgumentItem argument)
        {
            if (argument.Value == null)
                return string.Empty;

            var encryptedValue = argument.Value.ToString()!;

            if (encryptedValue == "EMPTY_STRING_MARKER")
                return "";

            lock (_lock)
            {
                if (_encryptionMap.TryGetValue(encryptedValue, out var original))
                    return original;
            }

            try
            {
                var bytes = Convert.FromBase64String(encryptedValue);
                var decoded = Encoding.UTF8.GetString(bytes);

                lock (_lock)
                {
                    if (!_encryptionMap.ContainsKey(encryptedValue))
                        _encryptionMap[encryptedValue] = decoded;
                }

                return decoded;
            }
            catch
            {
                return encryptedValue;
            }
        }
        public Task EncryptSensitiveArgumentsAsync(IScheduleArguments arguments, CancellationToken cancellationToken = default)
        {
            if (arguments == null)
                throw new ArgumentNullException(nameof(arguments));

            foreach (var item in arguments.Items.Where(x => x.IsEncryptedArgument))
            {
                var encrypted = EncryptArgument(item);
                item.Value = encrypted;
            }

            return Task.CompletedTask;
        }

        public Task DecryptSensitiveArgumentsAsync(IScheduleArguments arguments, CancellationToken cancellationToken = default)
        {
            if (arguments == null)
                throw new ArgumentNullException(nameof(arguments));

            foreach (var item in arguments.Items.Where(x => x.IsEncryptedArgument))
            {
                var decrypted = DecryptArgument(item);
                item.Value = decrypted;
            }

            return Task.CompletedTask;
        }

        public Task ReencryptSensitiveArgumentsAsync(IScheduleArguments arguments, CancellationToken ct = default)
        {
            if (arguments == null)
                throw new ArgumentNullException(nameof(arguments));

            foreach (var item in arguments.Items.Where(x => x.IsEncryptedArgument))
            {
                // Получаем оригинальное значение (если оно было расшифровано)
                // или расшифровываем текущее
                string originalValue;

                // Проверяем, является ли текущее значение зашифрованным
                try
                {
                    var bytes = Convert.FromBase64String(item.Value.ToString()!);
                    originalValue = Encoding.UTF8.GetString(bytes);
                }
                catch
                {
                    // Если не удалось декодировать, значит значение уже расшифровано
                    originalValue = item.Value.ToString()!;
                }

                // Шифруем заново
                var tempItem = new ScheduleArgumentItem
                {
                    Key = item.Key,
                    Value = originalValue,
                    IsEncryptedArgument = true
                };

                item.Value = EncryptArgument(tempItem);
            }

            return Task.CompletedTask;
        }
    }
}
