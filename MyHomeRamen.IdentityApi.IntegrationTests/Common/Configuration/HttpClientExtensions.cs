using System.Net.Http.Json;
using System.Text.Json;

namespace MyHomeRamen.IdentityApi.IntegrationTests.Common.Configuration;

internal static class HttpClientExtensions
{
    private static JsonSerializerOptions JsonOptions { get; } = new()
    {
        PropertyNameCaseInsensitive = true
    };

    internal static HttpRequestMessage CreateGetMessage(string url)
    {
        return new(HttpMethod.Get, url);
    }

    internal static HttpRequestMessage CreatePostMessage(string url)
    {
        return new(HttpMethod.Post, url);
    }

    internal static HttpRequestMessage CreateDeleteMessage(string url)
    {
        return new(HttpMethod.Delete, url);
    }

    internal static HttpRequestMessage WithJsonContent<T>(this HttpRequestMessage requestMessage, T body)
    {
        requestMessage.Content = JsonContent.Create(body);
        return requestMessage;
    }

    internal static async Task<TDto> ResponseToDto<TDto>(this HttpResponseMessage responseMessage)
    {
        string responseContent = await responseMessage.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        return JsonSerializer.Deserialize<TDto>(responseContent, JsonOptions)!;
    }
}
