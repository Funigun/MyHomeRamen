using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Domain.Identity.Permissions;
using MyHomeRamen.Features.Identity.Features.Users.GetDetails;
using MyHomeRamen.IdentityApi.IntegrationTests.Common;
using MyHomeRamen.IntegrationTests.Extensions;

namespace MyHomeRamen.IdentityApi.IntegrationTests.IdentityModule.Account;

public sealed class GetDetailsTests(IdentityWebApiFactory apiFactory) : IClassFixture<IdentityWebApiFactory>
{
    private const string Endpoint = "/api/account/me";
    private readonly IEnumerable<string> _requiredPermissions = [PermissionConstants.CanViewUserProfile];

    [Fact]
    public async Task GetDetails_ShouldReturn200_WithCorrectUserDetails()
    {
        // Arrange
        (string KeycloakId, Guid UserId) userId = await apiFactory.IdentityTestData.SeedUser(("Customer", _requiredPermissions), "Test", "Test");

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage(Endpoint);
        httpRequest.AddAuthorizationHeader(userId);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        GetDetailsResponse? body = await response.Content.ReadFromJsonAsync<GetDetailsResponse>(TestContext.Current.CancellationToken);
        Assert.NotNull(body);
        Assert.Equal("Test", body.Username);
        Assert.Equal("Test", body.FirstName);
        Assert.Equal("User", body.LastName);
        Assert.Equal($"Test@example.com", body.Email);
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
