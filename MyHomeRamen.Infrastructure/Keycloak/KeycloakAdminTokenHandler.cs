using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Options;
using MyHomeRamen.Infrastructure.Cache;

namespace MyHomeRamen.Infrastructure.Keycloak;

internal sealed class KeycloakAdminTokenHandler(
    IHttpClientFactory httpClientFactory,
    ICacheService cacheService,
    IOptions<KeycloakAdminOptions> options) : DelegatingHandler
{
    private readonly KeycloakAdminOptions _options = options.Value;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string accessToken = await GetOrFetchTokenAsync(cancellationToken);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return await base.SendAsync(request, cancellationToken);
    }

    private Task<string> GetOrFetchTokenAsync(CancellationToken cancellationToken) =>
        cacheService.GetOrSetAsync(
            new KeycloakAdminTokenCachePolicy(_options.TokenLifetimeSeconds),
            FetchTokenFromKeycloakAsync,
            cancellationToken);

    private async ValueTask<string> FetchTokenFromKeycloakAsync(CancellationToken cancellationToken)
    {
        // Uses a plain HttpClient (not the typed one) to avoid circular dependency
        using HttpClient client = httpClientFactory.CreateClient();
        string tokenUrl = $"{_options.BaseUrl}/realms/{_options.Realm}/protocol/openid-connect/token";

        using FormUrlEncodedContent form = new(new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials",
            ["client_id"] = _options.ClientId,
            ["client_secret"] = _options.ClientSecret,
        });

        using HttpResponseMessage response = await client.PostAsync(tokenUrl, form, cancellationToken);
        response.EnsureSuccessStatusCode();

        string json = await response.Content.ReadAsStringAsync(cancellationToken);
        using JsonDocument document = JsonDocument.Parse(json);

        return document.RootElement.GetProperty("access_token").GetString()!;
    }
}
