using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;

namespace MyHomeRamen.Identity.Api.Infrastructure;

internal sealed class KeycloakRolesClaimsTransformation : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity is not ClaimsIdentity { IsAuthenticated: true } identity)
        {
            return Task.FromResult(principal);
        }

        Claim? realmAccessClaim = principal.FindFirst("realm_access");

        if (realmAccessClaim is null)
        {
            return Task.FromResult(principal);
        }

        using JsonDocument document = JsonDocument.Parse(realmAccessClaim.Value);

        if (!document.RootElement.TryGetProperty("roles", out JsonElement roles))
        {
            return Task.FromResult(principal);
        }

        foreach (JsonElement role in roles.EnumerateArray())
        {
            string? roleValue = role.GetString();

            if (!string.IsNullOrEmpty(roleValue))
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, roleValue));
            }
        }

        return Task.FromResult(principal);
    }
}
