namespace MyHomeRamen.Blazor.Infrastructure.Api;

public static class HttpResponseMessageExtensions
{
    public static async Task EnsureApiSuccessAsync(
        this HttpResponseMessage response,
        string fallbackMessage,
        CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            throw await ApiRequestException.FromResponseAsync(response, fallbackMessage, cancellationToken);
        }
    }
}
