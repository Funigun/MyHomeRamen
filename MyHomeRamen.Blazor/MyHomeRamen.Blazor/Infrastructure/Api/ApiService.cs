using System.Net;
using Microsoft.AspNetCore.Components;

namespace MyHomeRamen.Blazor.Infrastructure.Api;

public sealed class ApiService(NavigationManager navigationManager, ILogger<ApiService> logger)
{
    public async Task<ApiResult<T>> ExecuteAsync<T>(Func<Task<T>> action)
    {
        try
        {
            return new ApiResult<T>.Success(await action());
        }
        catch (ApiException exception)
        {
            return Handle<T>(exception);
        }
        catch (HttpRequestException exception)
        {
            return HandleNetworkFailure<T>(exception);
        }
    }

    public async Task<ApiResult<bool>> ExecuteAsync(Func<Task> action)
    {
        return await ExecuteAsync<bool>(async () =>
        {
            await action();
            return true;
        });
    }

    public ApiResult<T> Handle<T>(ApiException exception)
    {
        bool handledGlobally = exception.StatusCode == HttpStatusCode.Forbidden || (int)exception.StatusCode >= 500;

        if (exception.StatusCode == HttpStatusCode.Forbidden)
        {
            navigationManager.NavigateTo("/forbidden");
        }
        else if ((int)exception.StatusCode >= 500)
        {
            navigationManager.NavigateTo("/Error");
        }

        ApiError error = new(
            exception.StatusCode,
            exception.Message,
            exception.ValidationErrors,
            exception.GeneralError,
            handledGlobally);

        return new ApiResult<T>.Failure(error);
    }

    private ApiResult<T> HandleNetworkFailure<T>(HttpRequestException exception)
    {
        logger.LogError(exception, "API request failed before receiving a response.");
        ApiError error = new(
            null,
            "The request could not be completed.",
            new Dictionary<string, string[]>(),
            null,
            false,
            true);

        return new ApiResult<T>.Failure(error);
    }
}
