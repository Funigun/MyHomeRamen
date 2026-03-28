using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MyHomeRamen.Domain.Users;

namespace MyHomeRamen.Identity.Api.Application.Services;

public class AuthorizationService
{
    public DateTime CalculateTokenExpirationTime(IConfiguration configuration, bool isRefreshToken)
    {
        int tokenDuration = isRefreshToken ? Convert.ToInt32(configuration["JwtSettings:RefreshTokenDurationInDays"]) : Convert.ToInt32(configuration["JwtSettings:Duration"]);

        return isRefreshToken ? DateTime.UtcNow.AddDays(tokenDuration) : DateTime.UtcNow.AddMinutes(tokenDuration);
    }

    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    }

    public async Task<string> GenerateToken(User user, UserManager<User> userManager, IConfiguration configuration, bool isRefreshToken, DateTime expirationTime)
    {
        string tokenKey = isRefreshToken ? configuration["JwtSettings:RefreshTokenKey"]! : configuration["JwtSettings:Key"]!;

        SymmetricSecurityKey? securitykey = new(Encoding.UTF8.GetBytes(tokenKey));
        SigningCredentials? credentials = new(securitykey, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken? token = new
        (
            issuer: configuration["JwtSettings:Issuer"],
            audience: configuration["JwtSettings:Audience"],
            claims: await GetClaims(user, userManager),
            expires: expirationTime,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<IList<Claim>> GetClaims(User user, UserManager<User> userManager)
    {
        IList<string> roles = await userManager.GetRolesAsync(user);
        IList<Claim> roleClaims = roles.Select(q => new Claim(ClaimTypes.Role, q)).ToList();

        IList<Claim> userClaims = await userManager.GetClaimsAsync(user);

        List<Claim> claims =
        [
            new(ClaimTypes.Name, user.UserName!),
            new("Id", user.Id.ToString()),
            new(JwtRegisteredClaimNames.Nickname, user.UserName!),
        ];

        claims.AddRange(userClaims);
        claims.AddRange(roleClaims);

        return claims;
    }
}
