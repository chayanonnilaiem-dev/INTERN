namespace DZ_MP.CORE.Utilities.Models;

public class RefreshTokenResult
{
    public string PlainToken { get; set; } = null!;
    public string TokenHash { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public int ExpiresInSeconds { get; set; }
}
