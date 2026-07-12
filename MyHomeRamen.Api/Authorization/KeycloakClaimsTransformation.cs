using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Identity.Abstractions;

namespace MyHomeRamen.Api.Authorization;

public sealed class KeycloakClaimsTransformation(IIdentityDbContext usersDbContext) : IClaimsTransformation
{
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity is not ClaimsIdentity { IsAuthenticated: true } identity)
        {
            return principal;
        }

        await SetUserIdClaim(principal, identity);
        TransformRoles(principal, identity);

        return principal;
    }

    private static void TransformRoles(ClaimsPrincipal principal, ClaimsIdentity identity)
    {
        Claim? resourceAccessClaim = principal.FindFirst("resource_access");

        if (resourceAccessClaim != null)
        {
            using JsonDocument document = JsonDocument.Parse(resourceAccessClaim.Value);
            foreach (JsonProperty client in document.RootElement.EnumerateObject())
            {
                if (client.Value.TryGetProperty("roles", out JsonElement clientRoles))
                {
                    foreach (JsonElement role in clientRoles.EnumerateArray())
                    {
                        string? roleValue = role.GetString();
                        if (!string.IsNullOrEmpty(roleValue))
                        {
                            identity.AddClaim(new Claim(ClaimTypes.Role, roleValue));
                        }
                    }
                }
            }

            identity.RemoveClaim(resourceAccessClaim);
        }
    }

    private async Task SetUserIdClaim(ClaimsPrincipal principal, ClaimsIdentity identity)
    {
        Claim? keycloakId = principal.FindFirst(ClaimConstants.KeycloakIdClaim);

        if (keycloakId != null)
        {
            Guid userId = (await usersDbContext.User.Query().GetIdByKeycloakId(keycloakId.Value, CancellationToken.None))!.Value;
            
            Claim? domainIdClaim = identity.Claims.FirstOrDefault(claim => claim.Type == ClaimConstants.DomainIdClaim);

            if (domainIdClaim != null)
            {
                identity.RemoveClaim(domainIdClaim);
            }

            identity.AddClaim(new Claim(ClaimConstants.DomainIdClaim, userId.ToString()));
        }
    }
}
