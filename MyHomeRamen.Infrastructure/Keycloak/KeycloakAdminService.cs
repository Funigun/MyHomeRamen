using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using MyHomeRamen.Features.Users.Services;
using MyHomeRamen.Features.Users.Services.Dto;
using MyHomeRamen.Infrastructure.Keycloak.Constants;

namespace MyHomeRamen.Infrastructure.Keycloak;

internal sealed class KeycloakAdminService(HttpClient httpClient, IOptions<KeycloakAdminOptions> adminOptions) : IKeycloakAdminService
{
    private readonly KeycloakAdminOptions _adminOptions = adminOptions.Value;

    public async Task<string> CreateUserAsync(KeycloakUserDto user, string roleName, CancellationToken cancellationToken = default)
    {
        IEnumerable<string> rolesToAdd = KeycloakRoleConstants.RoleMappings[roleName];

        string url = $"{_adminOptions.BaseUrl}/admin/realms/{_adminOptions.Realm}/users";

        using HttpResponseMessage response = await httpClient.PostAsJsonAsync(url, user, cancellationToken);

        response.EnsureSuccessStatusCode();

        string userId = response.Headers.Location.ToString().Split("/").Last();

        IEnumerable<KeycloakRoleDto> availableRoles = await GetAvailableRoles(cancellationToken);
        await AssignRolesToUser(userId, availableRoles.Where(r => rolesToAdd.Contains(r.Name)), cancellationToken);

        return userId;
    }

    public async Task AssignRolesToUser(string userId, IEnumerable<KeycloakRoleDto> roles, CancellationToken cancellationToken = default)
    {
        string clientId = await GetClientId(cancellationToken);

        string url = $"{_adminOptions.BaseUrl}/admin/realms/{_adminOptions.Realm}/users/{userId}/role-mappings/clients/{clientId}";

        using HttpResponseMessage response = await httpClient.PostAsJsonAsync(url, roles, cancellationToken);
    }

    public async Task<IEnumerable<KeycloakRoleDto>> GetAvailableRoles(CancellationToken cancellationToken = default)
    {
        string clientId = await GetClientId(cancellationToken);

        string url = $"{_adminOptions.BaseUrl}/admin/realms/{_adminOptions.Realm}/clients/{clientId}/roles";

        using HttpResponseMessage response = await httpClient.GetAsync(url, cancellationToken);

        IEnumerable<KeycloakRoleDto> roles = await response.Content.ReadFromJsonAsync<IEnumerable<KeycloakRoleDto>>(cancellationToken) ?? [];

        return roles.Where(role => KeycloakRoleConstants.AllRoles.Contains(role.Name));
    }

    public async Task<IEnumerable<KeycloakUserDto>> GetEmployees(CancellationToken cancellationToken = default)
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
