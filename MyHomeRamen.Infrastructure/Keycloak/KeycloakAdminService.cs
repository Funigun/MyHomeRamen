using System.Net.Http.Json;
using Microsoft.Extensions.Options;

namespace MyHomeRamen.Infrastructure.Keycloak;

internal sealed class KeycloakAdminService(HttpClient httpClient, IOptions<KeycloakAdminOptions> adminOptions) : IKeycloakAdminService
{
    private readonly KeycloakAdminOptions _dminOptions = adminOptions.Value;

    public async Task CreateUserAsync(KeycloakUserRepresentation user, CancellationToken cancellationToken = default)
    {
        string url = $"{_dminOptions.BaseUrl}/admin/realms/{_dminOptions.Realm}/users";

        using HttpResponseMessage response = await httpClient.PostAsJsonAsync(url, user, cancellationToken);

        response.EnsureSuccessStatusCode();
    }
}
