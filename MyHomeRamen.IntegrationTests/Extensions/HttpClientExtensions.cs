using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using MyHomeRamen.IntegrationTests.Authentication;

namespace MyHomeRamen.IntegrationTests.Extensions;

public static class HttpClientExtensions
{
    // Mirror scheme names from AuthorizationConfiguration in the API project
    private const string CustomerScheme = "RestaurantCustomer";
    private const string EmployeeScheme = "RestaurantEmployee";
    private const string ManagerScheme = "RestaurantManager";
    private const string AuthenticatedUserScheme = "AuthenticatedUser";
    private const string SchemeHeader = "x-scheme";

    private static JsonSerializerOptions JsonOptions { get; } = new()
    {
        PropertyNameCaseInsensitive = true
    };

    extension(HttpRequestMessage httpRequest)
    {
        public static HttpRequestMessage CreateGetMessage(string url) => new(HttpMethod.Get, url);

        public static HttpRequestMessage CreatePostMessage(string url) => new(HttpMethod.Post, url);

        public static HttpRequestMessage CreateDeleteMessage(string url) => new(HttpMethod.Delete, url);

        public static HttpRequestMessage CreatePutMessage(string url) => new(HttpMethod.Put, url);

        public HttpRequestMessage AddAuthorizationHeader((string keycloakUserId, Guid userId) user)
        {
            string token = JwtTokenFactory.GenerateToken(user.userId, user.keycloakUserId);

            httpRequest.Headers.Remove("Authorization");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return httpRequest;
        }

        public HttpRequestMessage AddAuthorizationHeader((Guid userId, Guid guestId) guest)
        {
            string token = JwtTokenFactory.GenerateGuestToken(guest.guestId);

            httpRequest.Headers.Remove("Authorization");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return httpRequest;
        }

        public HttpRequestMessage WithGuestCookie(string guestId)
        {
            httpRequest.Headers.Add("Cookie", $"guest_id={guestId}");
            return httpRequest;
        }

        public HttpRequestMessage WithJsonContent<T>(T body)
        {
            httpRequest.Content = JsonContent.Create(body);
            return httpRequest;
        }
    }

    extension(HttpResponseMessage httpResponse)
    {
        public async Task AssertStatusCode(HttpStatusCode expected)
        {
            if (httpResponse.StatusCode == expected)
            {
                return;
            }

            string content = await httpResponse.ReadMessageContent();
            Assert.Fail($"Expected status code {expected} but got {httpResponse.StatusCode}. Response body: {content}");
        }

        public async Task<string> ReadMessageContent() => await httpResponse.Content?.ReadAsStringAsync(TestContext.Current.CancellationToken) ?? string.Empty;

        public async Task<TDto> ResponseToDto<TDto>()
        {
            string responseContent = await httpResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            return JsonSerializer.Deserialize<TDto>(responseContent, JsonOptions)!;
        }
    }
}
