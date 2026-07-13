using System.Security.Cryptography;
using System.Text;

namespace DZ_MP.CORE.Utilities.Impl;

/// <summary>
/// AES symmetric encryption service — ลงทะเบียนด้วยมือ (ไม่ให้ Scrutor auto-scan เหมือน yota ตัดชื่อ AesEncryptionService)
/// </summary>
public class AesEncryptionService(string key) : ISymmetricEncryptionService
{
    private readonly byte[] _key = Convert.FromBase64String(key);

    public string Decrypt(string cipherText)
    {
        var fullCipher = Convert.FromBase64String(cipherText);
        using var aes = Aes.Create();
        aes.Key = _key;

        var iv = fullCipher.Take(16).ToArray();
        var cipher = fullCipher.Skip(16).ToArray();

        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        var plainBytes = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);

        return Encoding.UTF8.GetString(plainBytes);
    }

    public string Encrypt(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        var result = aes.IV.Concat(cipherBytes).ToArray();
        return Convert.ToBase64String(result);
    }
}
