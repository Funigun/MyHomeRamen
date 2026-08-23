using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Features.Identity.Features.Users.GetDetails;
using MyHomeRamen.IdentityApi.IntegrationTests.Common;
using MyHomeRamen.IntegrationTests.Authentication;
using MyHomeRamen.IntegrationTests.Extensions;

namespace MyHomeRamen.IdentityApi.IntegrationTests.IdentityModule.Account;

public sealed class GetDetailsTests(IdentityWebApiFactory apiFactory) : IClassFixture<IdentityWebApiFactory>
{
    private const string Endpoint = "/api/account/me";

    [Fact]
    public async Task GetDetails_ShouldReturn200_WithCorrectUserDetails()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage(Endpoint);
        httpRequest.AddAuthorizationHeader(UserRoles.Customer);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        GetDetailsResponse? body = await response.Content.ReadFromJsonAsync<GetDetailsResponse>(TestContext.Current.CancellationToken);
        Assert.NotNull(body);
        Assert.Equal("testcustomer", body.Username);
        Assert.Equal("Test", body.FirstName);
        Assert.Equal("Customer", body.LastName);
        Assert.Equal("testcustomer@example.com", body.Email);
        Assert.Equal("123456789", body.PhoneNumber);
    }

    [Fact]
    public async Task GetDetails_ShouldReturn401_WhenUnauthenticated()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage(Endpoint);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
