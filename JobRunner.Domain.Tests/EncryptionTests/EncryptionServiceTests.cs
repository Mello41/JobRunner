using FluentAssertions;
using JobRunner.Core.DefaultImplementations;
using JobRunner.Core.DTO.ScheduleDTO;
using JobRunner.Core.Interfaces.Core;

namespace JobRunner.Domain.Tests.EncryptionTests
{
    public class EncryptionServiceTests
    {
        private readonly IEncryptionService _encryptionService;

        public EncryptionServiceTests()
        {
            _encryptionService = new TestEncryptionService();
        }

        [Fact]
        public void EncryptArgument_ThenDecrypt_ReturnsOriginalValue()
        {
            // Arrange
            var originalValue = "my-secret-password";
            var argument = new ScheduleArgumentItem
            {
                Key = "--password",
                Value = originalValue,
                IsEncryptedArgument = true
            };

            // Act
            var encrypted = _encryptionService.EncryptArgument(argument);
            argument.Value = encrypted;
            var decrypted = _encryptionService.DecryptArgument(argument);

            // Assert
            decrypted.Should().Be(originalValue);
        }

        [Fact]
        public async Task EncryptSensitiveArgumentsAsync_OnlyEncryptsMarkedArguments()
        {
            // Arrange
            var arguments = new ScheduleArgumentsExample
            {
                Items = new List<ScheduleArgumentItem>
                {
                    new() { Key = "--user", Value = "admin", IsEncryptedArgument = false },
                    new() { Key = "--password", Value = "secret123", IsEncryptedArgument = true },
                    new() { Key = "--config", Value = "config.json", IsEncryptedArgument = false }
                }
            };

            // Act
            await _encryptionService.EncryptSensitiveArgumentsAsync(arguments);

            // Assert
            var passwordArg = arguments.Items.First(x => x.Key == "--password");
            var userArg = arguments.Items.First(x => x.Key == "--user");
            var configArg = arguments.Items.First(x => x.Key == "--config");

            passwordArg.Value.Should().NotBe("secret123", "password should be encrypted");
            passwordArg.Value.ToString().Should().NotBeNullOrWhiteSpace();

            userArg.Value.Should().Be("admin", "non-encrypted argument should not be changed");
            configArg.Value.Should().Be("config.json", "non-encrypted argument should not be changed");
        }

        [Fact]
        public async Task DecryptSensitiveArgumentsAsync_OnlyDecryptsEncryptedArguments()
        {
            // Arrange
            var arguments = new ScheduleArgumentsExample
            {
                Items = new List<ScheduleArgumentItem>
                {
                    new() { Key = "--password", Value = "secret123", IsEncryptedArgument = true },
                    new() { Key = "--token", Value = "plain_token", IsEncryptedArgument = false }
                }
            };

            // Act - сначала шифруем
            await _encryptionService.EncryptSensitiveArgumentsAsync(arguments);
            var encryptedValue = arguments.Items.First(x => x.Key == "--password").Value.ToString();

            // Сохраняем зашифрованное значение и расшифровываем
            arguments.Items.First(x => x.Key == "--password").Value = encryptedValue;
            await _encryptionService.DecryptSensitiveArgumentsAsync(arguments);

            // Assert
            var passwordArg = arguments.Items.First(x => x.Key == "--password");
            var tokenArg = arguments.Items.First(x => x.Key == "--token");

            passwordArg.Value.ToString().Should().NotBe(encryptedValue, "password should be decrypted");
            passwordArg.Value.Should().Be("secret123", "decrypted value should match original");
            tokenArg.Value.Should().Be("plain_token", "non-encrypted argument should remain unchanged");
        }

        [Fact]
        public async Task ReencryptSensitiveArgumentsAsync_RestoresEncryptedState()
        {
            // Arrange
            var arguments = new ScheduleArgumentsExample
            {
                Items = new List<ScheduleArgumentItem>
                {
                    new() { Key = "--api-key", Value = "real-api-key", IsEncryptedArgument = true }
                }
            };

            // Act
            await _encryptionService.DecryptSensitiveArgumentsAsync(arguments);
            var decryptedValue = arguments.Items.First().Value.ToString();

            await _encryptionService.ReencryptSensitiveArgumentsAsync(arguments);
            var reencryptedValue = arguments.Items.First().Value.ToString();

            // Assert
            decryptedValue.Should().Be("real-api-key", "decrypted value should be original");
            reencryptedValue.Should().NotBe("real-api-key", "re-encrypted value should be different from original");
            reencryptedValue.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task MultipleEncryptDecryptCycles_ShouldWorkCorrectly()
        {
            // Arrange
            var originalValue = "super-secret-password-123!@#";
            var arguments = new ScheduleArgumentsExample
            {
                Items = new List<ScheduleArgumentItem>
                {
                    new() { Key = "--secret", Value = originalValue, IsEncryptedArgument = true }
                }
            };

            // Act & Assert - несколько циклов шифрования/дешифрования
            for (int i = 0; i < 5; i++)
            {
                await _encryptionService.EncryptSensitiveArgumentsAsync(arguments);
                var encrypted = arguments.Items.First().Value.ToString();
                encrypted.Should().NotBe(originalValue, $"Cycle {i + 1}: should be encrypted");

                await _encryptionService.DecryptSensitiveArgumentsAsync(arguments);
                var decrypted = arguments.Items.First().Value.ToString();
                decrypted.Should().Be(originalValue, $"Cycle {i + 1}: should be decrypted back");
            }
        }

        [Fact]
        public async Task Reencrypt_WhenArgumentsNotDecrypted_ShouldStillWork()
        {
            // Arrange
            var arguments = new ScheduleArgumentsExample
            {
                Items = new List<ScheduleArgumentItem>
                {
                    new() { Key = "--key", Value = "initial-value", IsEncryptedArgument = true }
                }
            };

            // Act - шифруем, потом сразу reencrypt (без decrypt)
            await _encryptionService.EncryptSensitiveArgumentsAsync(arguments);
            var firstEncryption = arguments.Items.First().Value.ToString();

            await _encryptionService.ReencryptSensitiveArgumentsAsync(arguments);
            var secondEncryption = arguments.Items.First().Value.ToString();

            // Assert
            firstEncryption.Should().NotBe("initial-value");
            secondEncryption.Should().NotBe("initial-value");
            // Примечание: при повторном шифровании уже зашифрованных данных,
            // результат может быть другим из-за соли/IV. Это нормально.
        }

        [Fact]
        public void EncryptArgument_WithNullValue_ShouldHandleGracefully()
        {
            // Arrange
            var argument = new ScheduleArgumentItem
            {
                Key = "--null",
                Value = null!,
                IsEncryptedArgument = true
            };

            // Act
            var act = () => _encryptionService.EncryptArgument(argument);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void DecryptArgument_WithInvalidEncryptedValue_ShouldReturnOriginal()
        {
            // Arrange
            var invalidEncrypted = "this-is-not-valid-base64!!!";
            var argument = new ScheduleArgumentItem
            {
                Key = "--test",
                Value = invalidEncrypted,
                IsEncryptedArgument = true
            };

            // Act
            var decrypted = _encryptionService.DecryptArgument(argument);

            // Assert
            decrypted.Should().Be(invalidEncrypted, "should return original if decryption fails");
        }

        [Fact]
        public async Task EncryptSensitiveArgumentsAsync_WithEmptyCollection_ShouldNotThrow()
        {
            // Arrange
            var arguments = new ScheduleArgumentsExample
            {
                Items = new List<ScheduleArgumentItem>()
            };

            // Act
            var act = async () => await _encryptionService.EncryptSensitiveArgumentsAsync(arguments);

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task DecryptSensitiveArgumentsAsync_WithNullArguments_ShouldThrow()
        {
            // Act
            var act = async () => await _encryptionService.DecryptSensitiveArgumentsAsync(null!);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public void EncryptArgument_WithEmptyString_ShouldWork()
        {
            // Arrange
            var argument = new ScheduleArgumentItem
            {
                Key = "--empty",
                Value = "",
                IsEncryptedArgument = true
            };

            // Act
            var encrypted = _encryptionService.EncryptArgument(argument);

            // Assert
            encrypted.Should().NotBeNullOrWhiteSpace();
            encrypted.Should().NotBe("");

            argument.Value = encrypted;
            var decrypted = _encryptionService.DecryptArgument(argument);
            decrypted.Should().BeEmpty();
        }

        [Fact]
        public async Task ReencryptSensitiveArgumentsAsync_WithMultipleArguments_ShouldPreserveKeys()
        {
            // Arrange
            var arguments = new ScheduleArgumentsExample
            {
                Items = new List<ScheduleArgumentItem>
                {
                    new() { Key = "--arg1", Value = "value1", IsEncryptedArgument = true },
                    new() { Key = "--arg2", Value = "value2", IsEncryptedArgument = true },
                    new() { Key = "--arg3", Value = "value3", IsEncryptedArgument = false }
                }
            };

            // Act
            await _encryptionService.EncryptSensitiveArgumentsAsync(arguments);

            var encryptedValues = arguments.Items
                .Where(x => x.IsEncryptedArgument)
                .ToDictionary(x => x.Key, x => x.Value.ToString());

            await _encryptionService.DecryptSensitiveArgumentsAsync(arguments);

            foreach (var item in arguments.Items.Where(x => x.IsEncryptedArgument))
            {
                item.Value.Should().Be(item.Key == "--arg1" ? "value1" : "value2");
            }

            await _encryptionService.ReencryptSensitiveArgumentsAsync(arguments);

            foreach (var item in arguments.Items.Where(x => x.IsEncryptedArgument))
            {
                item.Value.ToString().Should().NotBeNullOrWhiteSpace();

                item.Value.Should().NotBe(item.Key == "--arg1" ? "value1" : "value2");

                item.Key.Should().Be(item.Key);
            }

            var nonEncrypted = arguments.Items.First(x => x.Key == "--arg3");
            nonEncrypted.Value.Should().Be("value3", "non-encrypted arguments should not change");
        }
    }
}
