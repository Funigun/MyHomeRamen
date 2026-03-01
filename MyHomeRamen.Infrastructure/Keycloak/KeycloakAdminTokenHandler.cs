using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MyHomeRamen.Infrastructure.Cache;

namespace MyHomeRamen.Infrastructure.Keycloak;

internal sealed class KeycloakAdminTokenHandler(
    IHttpClientFactory httpClientFactory,
    ICacheService cacheService,
    IConfiguration configuration,
    IOptions<KeycloakAdminOptions> admninOptions) : DelegatingHandler
{
    private readonly KeycloakAdminOptions _adminOptions = admninOptions.Value;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string accessToken = await GetOrFetchTokenAsync(cancellationToken);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return await base.SendAsync(request, cancellationToken);
    }

    private Task<string> GetOrFetchTokenAsync(CancellationToken cancellationToken) =>
        cacheService.GetOrSetAsync(
            new KeycloakAdminTokenCachePolicy(_adminOptions.TokenLifetimeSeconds),
            FetchTokenFromKeycloakAsync,
            cancellationToken);

    private async ValueTask<string> FetchTokenFromKeycloakAsync(CancellationToken cancellationToken)
    {
        // Uses a plain HttpClient (not the typed one) to avoid circular dependency
        using HttpClient client = httpClientFactory.CreateClient();
        string tokenUrl = $"{_adminOptions.BaseUrl}/realms/{_adminOptions.Realm}/protocol/openid-connect/token";

        using FormUrlEncodedContent form = new(new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials",
            ["client_id"] = _adminOptions.ClientId,
            ["client_secret"] = _adminOptions.ClientSecret,
        });

        using HttpResponseMessage response = await client.PostAsync(tokenUrl, form, cancellationToken);
        response.EnsureSuccessStatusCode();

        string json = await response.Content.ReadAsStringAsync(cancellationToken);
        using JsonDocument document = JsonDocument.Parse(json);

        return document.RootElement.GetProperty("access_token").GetString()!;
    }
}
