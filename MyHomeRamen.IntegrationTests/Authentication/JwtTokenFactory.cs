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

    public static string GenerateAdminToken(string userId = "")
    {
        IEnumerable<Claim> claims = GenerateClaimsForRole(UserRoles.Admin, userId);

        return TokenHandler.WriteToken(new JwtSecurityToken(Issuer, Audience, claims, null, DateTime.UtcNow.AddMinutes(20), SigningCredentials));
    }

    public static string GenerateEmployeeToken(string userId = "")
    {
        IEnumerable<Claim> claims = GenerateClaimsForRole(UserRoles.Employee, userId);

        return TokenHandler.WriteToken(new JwtSecurityToken(Issuer, Audience, claims, null, DateTime.UtcNow.AddMinutes(20), SigningCredentials));
    }

    public static string GenerateCustomerToken(string userId = "")
    {
        IEnumerable<Claim> claims = GenerateClaimsForRole(UserRoles.Customer, userId);

        return TokenHandler.WriteToken(new JwtSecurityToken(Issuer, Audience, claims, null, DateTime.UtcNow.AddMinutes(20), SigningCredentials));
    }

    private static IEnumerable<Claim> GenerateClaimsForRole(UserRoles role, string userId)
    {
        List<Claim> claims = [];

        switch (role)
        {
            case UserRoles.Admin:
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                claims.Add(new Claim(ClaimTypes.Role, "MenuAdmin"));
                claims.Add(new Claim(ClaimTypes.Role, "ShoppingCartAdmin"));
                break;
            case UserRoles.Employee:
                claims.Add(new Claim(ClaimTypes.Role, "Employee"));
                claims.Add(new Claim(ClaimTypes.Role, "MenuEmployee"));
                claims.Add(new Claim(ClaimTypes.Role, "ShoppingCartEmployee"));
                break;
            default:
                claims.Add(new Claim(ClaimTypes.Role, "Customer"));
                claims.Add(new Claim(ClaimTypes.Role, "MenuCustomer"));
                claims.Add(new Claim(ClaimTypes.Role, "ShoppingCartCustomer"));
                break;
        }

        if (!string.IsNullOrEmpty(userId))
        {
            claims.Add(new Claim(ClaimConstants.DomainIdClaim, userId));
            claims.Add(new Claim(ClaimConstants.KeycloakIdClaim, userId));
        }

        return claims;
    }
}
