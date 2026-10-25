using System.Net;
using MyHomeRamen.Blazor.Infrastructure.Api;

namespace MyHomeRamen.Blazor.Features.Restaurants.Companies.Shared;

public sealed class SocialMediaApiClient(HttpClient httpClient, ApiService apiService)
{
    private const string BaseUrl = "api/restaurants/company/social-media";
    private const string FailureMessage = "Social media request failed.";

    public Task<ApiResult<GetSocialMediaForManageResponse>> GetForManageAsync(CancellationToken cancellationToken = default) =>
        apiService.ExecuteAsync(() => GetForManageCoreAsync(cancellationToken));

    public Task<ApiResult<bool>> CreateAsync(CreateSocialMediaRequest request, CancellationToken cancellationToken = default) =>
        apiService.ExecuteAsync(() => CreateCoreAsync(request, cancellationToken));

    public Task<ApiResult<bool>> UpdateAsync(Guid socialMediaId, UpdateSocialMediaRequest request, CancellationToken cancellationToken = default) =>
        apiService.ExecuteAsync(() => UpdateCoreAsync(socialMediaId, request, cancellationToken));

    public Task<ApiResult<bool>> DeleteAsync(Guid socialMediaId, CancellationToken cancellationToken = default) =>
        apiService.ExecuteAsync(() => DeleteCoreAsync(socialMediaId, cancellationToken));

    private async Task<GetSocialMediaForManageResponse> GetForManageCoreAsync(CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await httpClient.GetAsync($"{BaseUrl}/manage", cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);

        return await response.Content.ReadFromJsonAsync<GetSocialMediaForManageResponse>(cancellationToken: cancellationToken)
            ?? throw new ApiRequestException(HttpStatusCode.InternalServerError, FailureMessage);
    }

    private async Task CreateCoreAsync(CreateSocialMediaRequest request, CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await httpClient.PostAsJsonAsync(BaseUrl, request, cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);
    }

    private async Task UpdateCoreAsync(Guid socialMediaId, UpdateSocialMediaRequest request, CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{socialMediaId}", request, cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);
    }

    private async Task DeleteCoreAsync(Guid socialMediaId, CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await httpClient.DeleteAsync($"{BaseUrl}/{socialMediaId}", cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);
    }
}
