using System.Security.Claims;
using DZ_MP.CORE.Utilities.Models;

namespace DZ_MP.CORE.Utilities;

public interface ITokenService
{
    AccessTokenResult GenerateAccessToken(string username, string role, IEnumerable<Claim>? additionalClaims = null);
    RefreshTokenResult GenerateRefreshToken(bool isRememberMe);
}
