using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Options;
using MyHomeRamen.Features.Common.Cache;
using MyHomeRamen.Infrastructure.Cache;

namespace MyHomeRamen.Infrastructure.Keycloak;

internal sealed class KeycloakAdminTokenHandler(
    IHttpClientFactory httpClientFactory,
    ICacheService cacheService,
    IOptions<KeycloakAdminOptions> admninOptions) : DelegatingHandler
{
    private readonly KeycloakAdminOptions _adminOptions = admninOptions.Value;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string accessToken = await GetOrFetchTokenAsync(cancellationToken);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

        // Retry to get access token in case when Keycloak was restared and the cached token is no longer valid
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            await cacheService.RemoveByKeyAsync(
                "keycloak_admin_access_token",
                cancellationToken);

            accessToken = await GetOrFetchTokenAsync(cancellationToken);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            response = await base.SendAsync(request, cancellationToken);
        }

        return response;
    }

    private async Task<string> GetOrFetchTokenAsync(CancellationToken cancellationToken) 
        => await cacheService.GetOrSetAsync
        (
            CachePolicy.LocalCache<IdentityCacheModule>("", TimeSpan.FromSeconds(_adminOptions.TokenLifetimeSeconds), ["keycloak_admin_token"]),
            async (cancellationToken) => await FetchTokenFromKeycloakAsync(cancellationToken),
            cancellationToken
        );

    private async Task<string> FetchTokenFromKeycloakAsync(CancellationToken cancellationToken)
    {
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
