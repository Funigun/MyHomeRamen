using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.IdentityApi.IntegrationTests.Common.Configuration;

namespace MyHomeRamen.IdentityApi.IntegrationTests.Common;

internal static class IdentityJwtHelper
{
    private static readonly JwtSecurityTokenHandler TokenHandler = new();

    internal static HttpRequestMessage AddIdentityAuthorizationHeader(
        this HttpRequestMessage requestMessage,
        string keycloakUserId,
        string role = "customer",
        string scheme = "RestaurantCustomer")
    {
        List<Claim> claims =
        [
            new Claim(ClaimConstants.KeycloakIdClaim, keycloakUserId),
            new Claim(ClaimTypes.Role, role)
        ];

        string token = TokenHandler.WriteToken(new JwtSecurityToken(
            JwtTokenFactory.Issuer,
            JwtTokenFactory.Audience,
            claims,
            null,
            DateTime.UtcNow.AddMinutes(20),
            JwtTokenFactory.SigningCredentials));

        requestMessage.Headers.Authorization = new("Bearer", token);
        requestMessage.Headers.Remove("x-scheme");
        requestMessage.Headers.Add("x-scheme", scheme);

        return requestMessage;
    }
}
