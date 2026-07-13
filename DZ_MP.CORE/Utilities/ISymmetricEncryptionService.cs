namespace DZ_MP.CORE.Utilities;

public interface ISymmetricEncryptionService
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
}
