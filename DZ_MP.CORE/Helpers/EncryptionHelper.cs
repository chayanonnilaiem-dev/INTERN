using System.Security.Cryptography;
using System.Text;

namespace DZ_MP.CORE.Helpers;

/// <summary>
/// Static helper สำหรับ encrypt/decrypt และ hash ข้อมูล sensitive
///
/// • Encrypt/Decrypt — AES-256-GCM (Authenticated Encryption)
/// • Hash            — HMAC-SHA256 ใช้สำหรับตรวจสอบข้อมูลซ้ำโดยไม่ต้อง decrypt
///
/// โดยใช้ hex key ขนาด 64 ตัวอักษร (32 bytes / 256-bit)
///
/// จัดเก็บ key ใน appsettings.json / Environment Variable เท่านั้น ห้าม hardcode ใน source code
/// </summary>
public static class EncryptionHelper
{
    private const int NonceSize = 12; // 96-bit nonce (GCM recommended)
    private const int TagSize = 16; // 128-bit authentication tag
    private const int KeySize = 32; // AES-256

    public static string Encrypt(string plainText, string hexKey)
    {
        var key = ParseHexKey(hexKey);
        var plainBytes = Encoding.UTF8.GetBytes(plainText);

        var nonce = new byte[NonceSize];
        var tag = new byte[TagSize];
        var cipherBytes = new byte[plainBytes.Length];

        RandomNumberGenerator.Fill(nonce);

        using var aesGcm = new AesGcm(key, TagSize);
        aesGcm.Encrypt(nonce, plainBytes, cipherBytes, tag);

        var output = new byte[NonceSize + TagSize + cipherBytes.Length];
        nonce.CopyTo(output, 0);
        tag.CopyTo(output, NonceSize);
        cipherBytes.CopyTo(output, NonceSize + TagSize);

        return Convert.ToBase64String(output);
    }

    public static string Decrypt(string cipherText, string hexKey)
    {
        var key = ParseHexKey(hexKey);
        var fullData = Convert.FromBase64String(cipherText);

        if (fullData.Length <= NonceSize + TagSize)
            throw new ArgumentException("cipherText is too short.", nameof(cipherText));

        var nonce = fullData[..NonceSize];
        var tag = fullData[NonceSize..(NonceSize + TagSize)];
        var cipherBytes = fullData[(NonceSize + TagSize)..];
        var plainBytes = new byte[cipherBytes.Length];

        using var aesGcm = new AesGcm(key, TagSize);
        aesGcm.Decrypt(nonce, cipherBytes, tag, plainBytes);

        return Encoding.UTF8.GetString(plainBytes);
    }

    public static string Hash(string plainText, string hexKey)
    {
        var key = ParseHexKey(hexKey);
        var plainBytes = Encoding.UTF8.GetBytes(plainText);

        var hashBytes = HMACSHA256.HashData(key, plainBytes);

        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }

    private static byte[] ParseHexKey(string hexKey)
    {
        if (string.IsNullOrWhiteSpace(hexKey))
            throw new ArgumentException("Encryption key must not be empty.", nameof(hexKey));

        if (hexKey.Length != KeySize * 2)
            throw new ArgumentException(
                $"Hex key must be exactly {KeySize * 2} characters (got {hexKey.Length}).",
                nameof(hexKey));

        var key = new byte[KeySize];
        for (var i = 0; i < KeySize; i++)
            key[i] = Convert.ToByte(hexKey.Substring(i * 2, 2), 16);

        return key;
    }
}
