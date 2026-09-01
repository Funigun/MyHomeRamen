using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using MyHomeRamen.Features.Identity.Services;
using MyHomeRamen.Features.Identity.Services.Dto;

namespace MyHomeRamen.Infrastructure.Keycloak;

internal sealed class KeycloakAdminService(HttpClient httpClient, IOptions<KeycloakAdminOptions> adminOptions) : IKeycloakAdminService
{
    private readonly KeycloakAdminOptions _adminOptions = adminOptions.Value;

    public async Task<string> CreateUserAsync(KeycloakUserDto user, string roleName, CancellationToken cancellationToken)
    {
        string url = $"{_adminOptions.BaseUrl}/admin/realms/{_adminOptions.Realm}/users";

        using HttpResponseMessage response = await httpClient.PostAsJsonAsync(url, user, cancellationToken);

        response.EnsureSuccessStatusCode();

        string userId = response.Headers.Location.ToString().Split("/").Last();

        return userId;
    }

    public async Task<IEnumerable<KeycloakUserDto>> GetEmployees(CancellationToken cancellationToken)
    {
        const string employeeRoleName = "employee";

        string clientId = await GetClientId(cancellationToken);

        string url = $"{_adminOptions.BaseUrl}/admin/realms/{_adminOptions.Realm}/clients/{clientId}/roles/{employeeRoleName}/users";

        using HttpResponseMessage response = await httpClient.GetAsync(url, cancellationToken);

        return await response.Content.ReadFromJsonAsync<IEnumerable<KeycloakUserDto>>(cancellationToken) ?? [];
    }

    private async Task<string> GetClientId(CancellationToken cancellationToken)
    {
        string url = $"{_adminOptions.BaseUrl}/admin/realms/{_adminOptions.Realm}/clients?clientid=my-home-ramen-client";

        using HttpResponseMessage response = await httpClient.GetAsync(url, cancellationToken);

        IEnumerable<KeycloakClientDto> clients = await response.Content.ReadFromJsonAsync<IEnumerable<KeycloakClientDto>>(cancellationToken) ?? [];

        if (clients is null)
        {
            throw new InvalidOperationException($"Client with clientId '{_adminOptions.ClientId}' not found in Keycloak realm '{_adminOptions.Realm}'.");
        }

        return clients.First().Id;
    }
}
