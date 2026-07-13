using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DZ_MP.CORE.Utilities.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DZ_MP.CORE.Utilities.Impl;

public class TokenService(IConfiguration configuration, IHashService hashService) : ITokenService
{
    private readonly IHashService _hashService = hashService;
    private readonly string _key = configuration["JwtSettings:Key"] ?? throw new ArgumentNullException(nameof(configuration), "JwtSettings:Key not found");
    private readonly int _accessTokenMinutes = int.TryParse(configuration["JwtSettings:AccessTokenMinutes"], out var m) ? m : 15;
    private readonly int _refreshTokenDays = int.TryParse(configuration["JwtSettings:RefreshTokenDays"], out var d) ? d : 1;
    private readonly int _refreshTokenDaysRememberMe = int.TryParse(configuration["JwtSettings:RefreshTokenDaysRememberMe"], out var dr) ? dr : 30;

    public AccessTokenResult GenerateAccessToken(string username, string role, IEnumerable<Claim>? additionalClaims = null)
    {
        var claimsList = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            new Claim("role_name", role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (additionalClaims != null)
        {
            claimsList.AddRange(additionalClaims);
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiresAt = DateTime.UtcNow.AddMinutes(_accessTokenMinutes);

        var token = new JwtSecurityToken(
            issuer: configuration["JwtSettings:Issuer"],
            audience: configuration["JwtSettings:Audience"],
            claims: claimsList,
            expires: expiresAt,
            signingCredentials: creds
        );

        return new AccessTokenResult
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = expiresAt,
            ExpiresInSeconds = _accessTokenMinutes * 60
        };
    }

    public RefreshTokenResult GenerateRefreshToken(bool isRememberMe)
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        var plainToken = Convert.ToBase64String(randomBytes);
        var hash = _hashService.Hash(plainToken);

        var lifetimeDays = isRememberMe ? _refreshTokenDaysRememberMe : _refreshTokenDays;
        var expiresAt = DateTime.UtcNow.AddDays(lifetimeDays);

        return new RefreshTokenResult
        {
            PlainToken = plainToken,
            TokenHash = hash,
            ExpiresAt = expiresAt,
            ExpiresInSeconds = lifetimeDays * 24 * 60 * 60
        };
    }
}
