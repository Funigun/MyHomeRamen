using System.Net;
using MyHomeRamen.Domain.Identity.Permissions;
using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.IdentityApi.IntegrationTests.Common;
using MyHomeRamen.IntegrationTests.Extensions;

namespace MyHomeRamen.IdentityApi.IntegrationTests.IdentityModule.Addresses;

public sealed class DeleteAddressTests(IdentityWebApiFactory apiFactory) : IClassFixture<IdentityWebApiFactory>, IAsyncLifetime
{
    private readonly IEnumerable<string> _requiredPermissions = [PermissionConstants.CanViewUserProfile, PermissionConstants.CanUpdateUserProfile];

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
    public async Task DeleteAddress_ShouldReturn204_WhenAddressExists()
    {
        // Arrange
        Address address = Address.Create("123 Main St", "Building A", "Apt 1", "Cityville", "12345", isDefault: false);        
        User user = await apiFactory.IdentityDbContext.User.Load().ById(_userId.UserId, TestContext.Current.CancellationToken);
        user.AddAddress(address);
        await apiFactory.IdentityDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateDeleteMessage($"/api/account/me/addresses/{address.Id}");
        httpRequest.AddAuthorizationHeader(_userId);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAddress_ShouldReturn403_WhenAddressNotFound()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateDeleteMessage($"/api/account/me/addresses/{Guid.NewGuid()}");
        httpRequest.AddAuthorizationHeader(_userId);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAddress_ShouldReturn403_WhenAddressBelongsToAnotherUser()
    {
        // Arrange
        (string KeycloakId, Guid UserId) otherUserId = await apiFactory.IdentityTestData.SeedUser(("Customer", _requiredPermissions), "Test", "Test");
        Address address = Address.Create("123 Main St", "Building A", "Apt 1", "Cityville", "12345", isDefault: false);        
        User user = await apiFactory.IdentityDbContext.User.Load().ById(otherUserId.UserId, TestContext.Current.CancellationToken);
        user.AddAddress(address);
        await apiFactory.IdentityDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateDeleteMessage($"/api/account/me/addresses/{address.Id}");
        httpRequest.AddAuthorizationHeader(_userId);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAddress_ShouldReturn401_WhenUnauthenticated()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateDeleteMessage($"/api/account/me/addresses/{Guid.NewGuid()}");

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
