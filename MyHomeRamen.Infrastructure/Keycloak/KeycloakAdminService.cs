using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using MyHomeRamen.Domain.Menu.Users;
using MyHomeRamen.Infrastructure.Keycloak.Dto;

namespace MyHomeRamen.Infrastructure.Keycloak;

internal sealed class KeycloakAdminService(HttpClient httpClient, IOptions<KeycloakAdminOptions> adminOptions) : IKeycloakAdminService
{
    private readonly KeycloakAdminOptions _dminOptions = adminOptions.Value;

    public async Task CreateUserAsync(KeycloakUserDto user, CancellationToken cancellationToken = default)
    {
        string url = $"{_dminOptions.BaseUrl}/admin/realms/{_dminOptions.Realm}/users";

        using HttpResponseMessage response = await httpClient.PostAsJsonAsync(url, user, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            string userId = response.Headers.Location.ToString().Split("/").Last();

            IEnumerable<KeycloakRoleDto> availableRoles = await GetAvailableRoles(cancellationToken);
            await AssignRolesToUser(userId, availableRoles.Where(r => r.Name == "employee"), cancellationToken);
        }

        response.EnsureSuccessStatusCode();
    }

    public async Task AssignRolesToUser(string userId, IEnumerable<KeycloakRoleDto> roles, CancellationToken cancellationToken = default)
    {
        string clientId = await GetClientId(cancellationToken);

        foreach (KeycloakRoleDto role in roles)
        {
            string url = $"{_dminOptions.BaseUrl}/admin/realms/{_dminOptions.Realm}/users/{userId}/role-mappings/clients/{clientId}";

            using HttpResponseMessage response = await httpClient.PostAsJsonAsync(url, roles, cancellationToken);
        }
    }

    public async Task<IEnumerable<KeycloakRoleDto>> GetAvailableRoles(CancellationToken cancellationToken = default)
    {
        string[] rolesToFilter = ["employee", "customer", "order-admin", "payment-admin", "shopping-cart-admin", "reservation-admin", "menu-admin"];

        string clientId = await GetClientId(cancellationToken);

        string url = $"{_dminOptions.BaseUrl}/admin/realms/{_dminOptions.Realm}/clients/{clientId}/roles";

        using HttpResponseMessage response = await httpClient.GetAsync(url, cancellationToken);

        IEnumerable<KeycloakRoleDto> roles = await response.Content.ReadFromJsonAsync<IEnumerable<KeycloakRoleDto>>(cancellationToken) ?? [];

        return roles.Where(role => rolesToFilter.Contains(role.Name));
    }

    public async Task<IEnumerable<KeycloakUserDto>> GetEmployees(CancellationToken cancellationToken = default)
    {
        const string employeeRoleName = "employee";

        string clientId = await GetClientId(cancellationToken);

        string url = $"{_dminOptions.BaseUrl}/admin/realms/{_dminOptions.Realm}/clients/{clientId}/roles/{employeeRoleName}/users";

        using HttpResponseMessage response = await httpClient.GetAsync(url, cancellationToken);

        return await response.Content.ReadFromJsonAsync<IEnumerable<KeycloakUserDto>>(cancellationToken) ?? [];
    }

    private async Task<string> GetClientId(CancellationToken cancellationToken)
    {
        string url = $"{_dminOptions.BaseUrl}/admin/realms/{_dminOptions.Realm}/clients?clientid=my-home-ramen-client";

        using HttpResponseMessage response = await httpClient.GetAsync(url, cancellationToken);

        IEnumerable<KeycloakClientDto> clients = await response.Content.ReadFromJsonAsync<IEnumerable<KeycloakClientDto>>(cancellationToken) ?? [];

        if (clients is null)
        {
            throw new InvalidOperationException($"Client with clientId '{_dminOptions.ClientId}' not found in Keycloak realm '{_dminOptions.Realm}'.");
        }

        return clients.First().Id;
    }
}
