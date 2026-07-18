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

        public HttpRequestMessage AddAuthorizationHeader(UserRoles userRole, string userId = "")
        {
            (string token, string scheme) = userRole switch
            {
                UserRoles.Admin => (JwtTokenFactory.GenerateAdminToken(userId), ManagerScheme),
                UserRoles.Employee => (JwtTokenFactory.GenerateEmployeeToken(userId), EmployeeScheme),
                _ => (JwtTokenFactory.GenerateCustomerToken(userId), CustomerScheme)
            };

            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            httpRequest.Headers.Add(SchemeHeader, scheme);

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
