using System.Threading.Tasks;

namespace JobRunner.Core.Interfaces
{
    /// <summary>
    /// Интерфейс для обратимого шифрования данных
    /// </summary>
    public interface IStringEncryptor
    {
        string Decrypt(string plantext);
        string Encrypt(string cipherText);
    }
}
