namespace DZ_MP.CORE.Utilities.Models;

public class AccessTokenResult
{
    public string Token { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public int ExpiresInSeconds { get; set; }
}
