using System.Net.Http.Json;
using Microsoft.Extensions.Options;

namespace MyHomeRamen.Infrastructure.Keycloak;

internal sealed class KeycloakAdminService(
    HttpClient httpClient,
    IOptions<KeycloakAdminOptions> options) : IKeycloakAdminService
{
    private readonly KeycloakAdminOptions _options = options.Value;

    public async Task CreateUserAsync(KeycloakUserRepresentation user, CancellationToken cancellationToken = default)
    {
        string url = $"{_options.BaseUrl}/admin/realms/{_options.Realm}/users";
        using HttpResponseMessage response = await httpClient.PostAsJsonAsync(url, user, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
