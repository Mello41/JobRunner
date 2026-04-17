using JobRunner.Core.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace JobRunner.Jobs.Encryption
{
    /// <summary>
    /// Реализация шифрования "IStringEncryptor"
    /// </summary>
    public class AesStringEncryptor : IStringEncryptor
    {
        private byte[] _key;
        private byte[] _iv;

        private readonly string _appDataFolder;
        private readonly string _keyFile;

        public AesStringEncryptor()
        {
            _appDataFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "JobRunner");

            _keyFile = Path.Combine(_appDataFolder, "encryption.bin");

            LoadOrCreateKey();
        }

        private void LoadOrCreateKey()
        {
            if (!Directory.Exists(_appDataFolder))
                Directory.CreateDirectory(_appDataFolder);

            if (File.Exists(_keyFile))
            {
                var protectedData = File.ReadAllBytes(_keyFile);
                var unprotectedData = ProtectedData.Unprotect(protectedData, null, DataProtectionScope.CurrentUser);

                _key = new byte[32];
                _iv = new byte[16];
                Array.Copy(unprotectedData, 0, _key, 0, 32);
                Array.Copy(unprotectedData, 32, _iv, 0, 16);
            }
            else
            {
                using var aes = Aes.Create();
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.GenerateKey();
                aes.GenerateIV();

                _key = aes.Key;
                _iv = aes.IV;

                var combined = new byte[48]; // 32 + 16
                Array.Copy(_key, 0, combined, 0, 32);
                Array.Copy(_iv, 0, combined, 32, 16);

                var protectedData = ProtectedData.
                    Protect(combined, null, DataProtectionScope.CurrentUser);

                File.WriteAllBytes(_keyFile, protectedData);
            }
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return string.Empty;

            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var encryptor = aes.CreateEncryptor();
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            return Convert.ToBase64String(cipherBytes);
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return string.Empty;

            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var decryptor = aes.CreateDecryptor();
            var cipherBytes = Convert.FromBase64String(cipherText);
            var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

            return Encoding.UTF8.GetString(plainBytes);
        }

        /// <summary>
        /// Сбросить ключ (создать новый при следующем запуске)
        /// </summary>
        public void ResetKey()
        {
            if (File.Exists(_keyFile))
                File.Delete(_keyFile);

            LoadOrCreateKey();
        }
    }
}
