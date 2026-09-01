using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using MyHomeRamen.Features.Identity.Services;
using MyHomeRamen.Features.Identity.Services.Dto;

namespace MyHomeRamen.Infrastructure.Keycloak;

internal sealed class KeycloakAdminService(HttpClient httpClient, IOptions<KeycloakAdminOptions> adminOptions) : IKeycloakAdminService
{
    private readonly KeycloakAdminOptions _adminOptions = adminOptions.Value;

    public async Task<string> CreateUserAsync(KeycloakUserDto user, CancellationToken cancellationToken)
    {
        string url = $"{_adminOptions.BaseUrl}/admin/realms/{_adminOptions.Realm}/users";

        using HttpResponseMessage response = await httpClient.PostAsJsonAsync(url, user, cancellationToken);

        response.EnsureSuccessStatusCode();

        string userId = response.Headers.Location.ToString().Split("/").Last();

        return userId;
    }
}
