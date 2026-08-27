using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Features.Identity.Features.Users.GetAddresses;
using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.IdentityApi.IntegrationTests.Common;
using MyHomeRamen.IntegrationTests.Extensions;
using MyHomeRamen.Domain.Identity.Permissions;

namespace MyHomeRamen.IdentityApi.IntegrationTests.IdentityModule.Addresses;

public sealed class GetAddressesTests(IdentityWebApiFactory apiFactory) : IClassFixture<IdentityWebApiFactory>, IAsyncLifetime
{
    private const string Endpoint = "/api/account/me/addresses";
    private readonly IEnumerable<string> _requiredPermissions = [PermissionConstants.CanViewUserProfile];

    private (string KeycloakId, Guid UserId) _userId;

    public async ValueTask InitializeAsync()
    {
        _userId = await apiFactory.IdentityTestData.SeedUser(("Customer", _requiredPermissions), "CustomerA", "Test");
    }

    public async ValueTask DisposeAsync()
    {
        await apiFactory.IdentityDbContext.User.ExecuteDelete(u => u.Id == new UserId(_userId.UserId), TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task GetAddresses_ShouldReturn200_WithAddressList()
    {
        // Arrange
        Address defaultAddress = Address.Create("123 Main St", "Building A", "Apt 1", "Cityville", "12345", isDefault: true);
        Address address = Address.Create("123 Main St", "Building A", "Apt 1", "Cityville", "12345", isDefault: false);

        User user = await apiFactory.IdentityDbContext.User.Load().ById(_userId.UserId, TestContext.Current.CancellationToken);
        user.AddAddress(defaultAddress);
        user.AddAddress(address);

        apiFactory.IdentityDbContext.User.Update(user);
        await apiFactory.IdentityDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage(Endpoint);
        httpRequest.AddAuthorizationHeader(_userId);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        GetAddressesResponse? body = await response.Content.ReadFromJsonAsync<GetAddressesResponse>(TestContext.Current.CancellationToken);
        Assert.NotNull(body);
        Assert.NotEmpty(body.Addresses);
        Assert.All(body.Addresses, a => Assert.NotEqual(Guid.Empty, a.Id));
    }

    [Fact]
    public async Task GetAddresses_ShouldReturn401_WhenUnauthenticated()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage(Endpoint);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAddresses_ShouldReturnEmptyList_WhenUserHasNoAddresses()
    {
        // Arrange        
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage(Endpoint);
        httpRequest.AddAuthorizationHeader(_userId);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        GetAddressesResponse? body = await response.Content.ReadFromJsonAsync<GetAddressesResponse>(TestContext.Current.CancellationToken);
        Assert.NotNull(body);
        Assert.Empty(body.Addresses);
    }
}
