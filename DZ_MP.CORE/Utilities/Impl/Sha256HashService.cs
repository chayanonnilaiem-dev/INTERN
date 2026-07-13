using System.Security.Cryptography;
using System.Text;

namespace DZ_MP.CORE.Utilities.Impl;

public class Sha256HashService : IHashService
{
    public string Hash(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes);
    }
}
