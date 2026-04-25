using MyHomeRamen.Blazor.Features.Account.Common.Models;
using MyHomeRamen.Blazor.Features.Account.SignUp;

namespace MyHomeRamen.Blazor.Features.Account.Common.Services;

public class CustomerAccountApiClient(HttpClient httpClient)
{
    public async Task<string> GetMyIdAsync(CancellationToken cancellationToken, string? bearerToken = null)
    {
        if (bearerToken is not null)
        {
            using HttpRequestMessage request = new(HttpMethod.Get, "/api/account/me/id");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);
            using HttpResponseMessage httpResponse = await httpClient.SendAsync(request, cancellationToken);
            httpResponse.EnsureSuccessStatusCode();
            GetMyIdResponse? result = await httpResponse.Content.ReadFromJsonAsync<GetMyIdResponse>(cancellationToken: cancellationToken);
            return result?.Id.ToString() ?? string.Empty;
        }

        GetMyIdResponse? response = await httpClient.GetFromJsonAsync<GetMyIdResponse>("/api/account/me/id", cancellationToken);
        return response?.Id.ToString() ?? string.Empty;
    }

    public async Task CreateAsync(SignUpRequest request, CancellationToken ct = default)
    {
        using HttpResponseMessage response = await httpClient.PostAsJsonAsync("/api/account/sign-up", request, ct);
        response.EnsureSuccessStatusCode();
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
