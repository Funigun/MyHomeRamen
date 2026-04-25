using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;

namespace MyHomeRamen.Blazor.Presentation.Authentication;

internal sealed class KeycloakRolesClaimsTransformation : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity is not ClaimsIdentity { IsAuthenticated: true } identity)
        {
            return Task.FromResult(principal);
        }

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

                        if (!string.IsNullOrEmpty(roleValue) && !identity.HasClaim(ClaimTypes.Role, roleValue))
                        {
                            identity.AddClaim(new Claim(ClaimTypes.Role, roleValue));
                        }
                    }
                }
            }
        }

        return Task.FromResult(principal);
    }
}
