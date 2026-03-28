using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace MyHomeRamen.IntegrationTests.Common.Configuration;

internal static class JwtTokenFactory
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

    public static string GenerateAdminToken()
    {
        List<Claim> claims =
        [
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim(ClaimTypes.Role, "MenuAdmin")
        ];

        return TokenHandler.WriteToken(new JwtSecurityToken(Issuer, Audience, claims, null, DateTime.UtcNow.AddMinutes(20), SigningCredentials));
    }

    public static string GenerateEmployeeToken()
    {
        List<Claim> claims =
        [
            new Claim(ClaimTypes.Role, "Employee"),
            new Claim(ClaimTypes.Role, "MenuEmployee")
        ];

        return TokenHandler.WriteToken(new JwtSecurityToken(Issuer, Audience, claims, null, DateTime.UtcNow.AddMinutes(20), SigningCredentials));
    }

    public static string GenerateCustomerToken()
    {
        List<Claim> claims =
        [
            new Claim(ClaimTypes.Role, "Customer"),
            new Claim(ClaimTypes.Role, "MenuCustomer")
        ];

        return TokenHandler.WriteToken(new JwtSecurityToken(Issuer, Audience, claims, null, DateTime.UtcNow.AddMinutes(20), SigningCredentials));
    }
}
