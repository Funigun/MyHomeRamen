using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using MyHomeRamen.Features.Common.Authorization;

namespace MyHomeRamen.IntegrationTests.Authentication;

public static class JwtTokenFactory
{
    public static string Issuer { get; } = Guid.NewGuid().ToString();

    public static string Audience { get; } = "test-audience";

    public static SecurityKey SecurityKey { get; }

    public static SigningCredentials SigningCredentials { get; }

    private static readonly JwtSecurityTokenHandler TokenHandler = new();
    private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();
    private static readonly byte[] Key = new byte[32];

    static JwtTokenFactory()
    {
        Rng.GetBytes(Key);
        SecurityKey = new SymmetricSecurityKey(Key) { KeyId = Guid.NewGuid().ToString() };
        SigningCredentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
    }

    public static string GenerateToken(Guid userId, string keycloakUserId)
    {
        IEnumerable<Claim> claims = [new Claim(ClaimConstants.DomainIdClaim, userId.ToString()),
                                                    new Claim(ClaimConstants.KeycloakIdClaim, keycloakUserId)];

        return TokenHandler.WriteToken(new JwtSecurityToken(Issuer, Audience, claims, null, DateTime.UtcNow.AddMinutes(20), SigningCredentials));
    }

    public static string GenerateAdminToken()
    {
        IEnumerable<Claim> claims = [];

        return TokenHandler.WriteToken(new JwtSecurityToken(Issuer, Audience, claims, null, DateTime.UtcNow.AddMinutes(20), SigningCredentials));
    }

    public static string GenerateEmployeeToken()
    {
        IEnumerable<Claim> claims = [];

        return TokenHandler.WriteToken(new JwtSecurityToken(Issuer, Audience, claims, null, DateTime.UtcNow.AddMinutes(20), SigningCredentials));
    }

    public static string GenerateCustomerToken()
    {
        IEnumerable<Claim> claims = [];

        return TokenHandler.WriteToken(new JwtSecurityToken(Issuer, Audience, claims, null, DateTime.UtcNow.AddMinutes(20), SigningCredentials));
    }

    public static string GenerateManagerToken()
    {
        IEnumerable<Claim> claims = [];

        return TokenHandler.WriteToken(new JwtSecurityToken(Issuer, Audience, claims, null, DateTime.UtcNow.AddMinutes(20), SigningCredentials));
    }
}
