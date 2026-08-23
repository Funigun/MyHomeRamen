using System.Net;
using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.IdentityApi.IntegrationTests.Common;
using MyHomeRamen.IntegrationTests.Extensions;

namespace MyHomeRamen.IdentityApi.IntegrationTests.IdentityModule.Addresses;

public sealed class DeleteAddressTests(IdentityWebApiFactory apiFactory) : IClassFixture<IdentityWebApiFactory>
{
    [Fact]
    public async Task DeleteAddress_ShouldReturn204_WhenAddressExists()
    {
        // Arrange
        Address address = Address.Create("123 Main St", "Building A", "Apt 1", "Cityville", "12345", isDefault: false);        
        User user = await apiFactory.IdentityDbContext.User.Load().ById(apiFactory.IdentityTestData.CustomerUser.UserId, TestContext.Current.CancellationToken);
        user.AddAddress(address);
        await apiFactory.IdentityDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateDeleteMessage($"/api/account/me/addresses/{address.Id}");
        httpRequest.AddAuthorizationHeader(apiFactory.IdentityTestData.CustomerUser);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAddress_ShouldReturn400_WhenAddressNotFound()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateDeleteMessage($"/api/account/me/addresses/{Guid.NewGuid()}");
        httpRequest.AddAuthorizationHeader(apiFactory.IdentityTestData.CustomerUser);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAddress_ShouldReturn400_WhenAddressBelongsToAnotherUser()
    {
        // Arrange
        Address address = Address.Create("123 Main St", "Building A", "Apt 1", "Cityville", "12345", isDefault: false);        
        User user = await apiFactory.IdentityDbContext.User.Load().ById(apiFactory.IdentityTestData.EmployeeUser.UserId, TestContext.Current.CancellationToken);
        user.AddAddress(address);
        await apiFactory.IdentityDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateDeleteMessage($"/api/account/me/addresses/{address.Id}");
        httpRequest.AddAuthorizationHeader(apiFactory.IdentityTestData.CustomerUser);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
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
