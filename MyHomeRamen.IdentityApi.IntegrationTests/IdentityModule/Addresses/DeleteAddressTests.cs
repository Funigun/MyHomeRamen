using System.Net;
using MyHomeRamen.IdentityApi.IntegrationTests.Common;
using MyHomeRamen.IdentityApi.IntegrationTests.Common.Configuration;
using MyHomeRamen.IdentityApi.IntegrationTests.Common.Data;

namespace MyHomeRamen.IdentityApi.IntegrationTests.IdentityModule.Addresses;

public sealed class DeleteAddressTests(IdentityWebApiFactory apiFactory)
{
    [Fact]
    public async Task DeleteAddress_ShouldReturn204_WhenAddressExists()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateDeleteMessage($"/api/account/me/addresses/{DataSeeder.SeededAddressId}")
            .AddIdentityAuthorizationHeader(DataSeeder.SeededUserKeycloakId);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAddress_ShouldReturn400_WhenAddressNotFound()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateDeleteMessage($"/api/account/me/addresses/{Guid.NewGuid()}")
            .AddIdentityAuthorizationHeader(DataSeeder.SeededUserKeycloakId);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAddress_ShouldReturn400_WhenAddressBelongsToAnotherUser()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateDeleteMessage($"/api/account/me/addresses/{DataSeeder.AnotherUserAddressId}")
            .AddIdentityAuthorizationHeader(DataSeeder.SeededUserKeycloakId);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAddress_ShouldReturn401_WhenUnauthenticated()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateDeleteMessage($"/api/account/me/addresses/{DataSeeder.SeededAddressId}");

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
