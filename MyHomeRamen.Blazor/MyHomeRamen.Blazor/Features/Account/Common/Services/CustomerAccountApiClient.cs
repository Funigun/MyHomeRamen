using MyHomeRamen.Blazor.Features.Account.Common.Models;
using MyHomeRamen.Blazor.Features.Account.SignUp;
using MyHomeRamen.Blazor.Features.Account.Common.Services.Contracts.Users.Account.Responses;

namespace MyHomeRamen.Blazor.Features.Account.Common.Services;

public class CustomerAccountApiClient(HttpClient httpClient)
{
    public async Task<GetMeModel> GetMeAsync(CancellationToken cancellationToken, string? bearerToken = null)
    {
        using HttpRequestMessage request = new(HttpMethod.Get, "/api/identity/users/me");
        if (!string.IsNullOrEmpty(bearerToken))
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);
        }

        using HttpResponseMessage httpResponse = await httpClient.SendAsync(request, cancellationToken);
        httpResponse.EnsureSuccessStatusCode();
        GetMeResponse response = await httpResponse.Content.ReadFromJsonAsync<GetMeResponse>(cancellationToken: cancellationToken)
                               ?? throw new InvalidOperationException("Current user response was empty.");

        return new GetMeModel(
            response.UserId,
            response.FirstName,
            new GetMeAdminNavigationModel(response.AdminNavigation.CanSeeAdminPanel, response.AdminNavigation.Sections));
    }

    public async Task CreateAsync(SignUpRequest request, CancellationToken ct = default)
    {
        using HttpResponseMessage response = await httpClient.PostAsJsonAsync("/api/account/sign-up", request, ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task<RegisterGuestResponse?> RegisterGuestAsync(CancellationToken ct = default)
    {
        using HttpResponseMessage response = await httpClient.PostAsync("/api/account/guest", null, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<RegisterGuestResponse>(cancellationToken: ct);
    }

    public async Task<GetDetailsResponse?> GetDetailsAsync(CancellationToken ct = default)
    {
        return await httpClient.GetFromJsonAsync<GetDetailsResponse>("/api/account/me", ct);
    }

    public async Task<GetAddressesResponse?> GetAddressesAsync(CancellationToken ct = default)
    {
        return await httpClient.GetFromJsonAsync<GetAddressesResponse>("/api/account/me/addresses", ct);
    }

    public async Task<Guid> AddAddressAsync(AddAddressRequest request, CancellationToken ct = default)
    {
        using HttpResponseMessage response = await httpClient.PostAsJsonAsync("/api/account/me/addresses", request, ct);
        response.EnsureSuccessStatusCode();
        AddAddressResponse? result = await response.Content.ReadFromJsonAsync<AddAddressResponse>(cancellationToken: ct);
        return result?.Id ?? Guid.Empty;
    }

    public async Task<UpdateAddressResponse?> UpdateAddressAsync(Guid addressId, UpdateAddressRequest request, CancellationToken ct = default)
    {
        using HttpResponseMessage response = await httpClient.PutAsJsonAsync($"/api/account/me/addresses/{addressId}", request, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UpdateAddressResponse>(cancellationToken: ct);
    }

    public async Task DeleteAddressAsync(Guid id, CancellationToken ct = default)
    {
        using HttpResponseMessage response = await httpClient.DeleteAsync($"/api/account/me/addresses/{id}", ct);
        response.EnsureSuccessStatusCode();
    }
}
