using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace MyHomeRamen.IdentityApi.IntegrationTests.Common.Configuration;

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
}
