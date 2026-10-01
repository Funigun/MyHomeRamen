using System.Net;

namespace MyHomeRamen.Blazor.Features.Restaurants.Companies.Shared;

public sealed class SocialMediaApiClient(HttpClient httpClient)
{
    private const string BaseUrl = "api/restaurants/company/social-media";

    public async Task<GetSocialMediaForManageResponse> GetForManageAsync(CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage response = await httpClient.GetAsync($"{BaseUrl}/manage", cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<GetSocialMediaForManageResponse>(cancellationToken: cancellationToken)
            ?? throw new SocialMediaApiException(HttpStatusCode.InternalServerError);
    }

    public async Task CreateAsync(CreateSocialMediaRequest request, CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage response = await httpClient.PostAsJsonAsync(BaseUrl, request, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
    }

    public async Task UpdateAsync(Guid socialMediaId, UpdateSocialMediaRequest request, CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{socialMediaId}", request, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
    }

    public async Task DeleteAsync(Guid socialMediaId, CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage response = await httpClient.DeleteAsync($"{BaseUrl}/{socialMediaId}", cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        string? detail = await response.Content.ReadAsStringAsync(cancellationToken);
        throw new SocialMediaApiException(response.StatusCode, detail);
    }
}

public sealed class SocialMediaApiException(HttpStatusCode statusCode, string? detail = null) : Exception(detail)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
}

