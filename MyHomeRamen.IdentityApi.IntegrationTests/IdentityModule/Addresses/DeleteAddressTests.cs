using System.Net;
using MyHomeRamen.IdentityApi.IntegrationTests.Common;
using MyHomeRamen.IdentityApi.IntegrationTests.Common.Configuration;

namespace MyHomeRamen.IdentityApi.IntegrationTests.IdentityModule.Addresses;

public sealed class DeleteAddressTests(IdentityApiFixture apiFixture) : IClassFixture<IdentityApiFixture>
{
    [Fact]
    public async Task DeleteAddress_ShouldReturn204_WhenAddressExists()
    {
        // Arrange
        using using HttpRequestMessage httpRequest = HttpClientExtensions.CreateDeleteMessage($"/api/account/me/addresses/{apiFixture.ApiFactory.DataSeeder.SeededAddressId}");
        httpRequest.AddIdentityAuthorizationHeader(apiFixture.ApiFactory.DataSeeder.SeededUserKeycloakId);


        // Act
        HttpResponseMessage response = await apiFixture.ApiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAddress_ShouldReturn400_WhenAddressNotFound()
    {
        // Arrange
        using using HttpRequestMessage httpRequest = HttpClientExtensions.CreateDeleteMessage($"/api/account/me/addresses/{Guid.NewGuid()}");
        httpRequest.AddIdentityAuthorizationHeader(apiFixture.ApiFactory.DataSeeder.SeededUserKeycloakId);


        // Act
        HttpResponseMessage response = await apiFixture.ApiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAddress_ShouldReturn400_WhenAddressBelongsToAnotherUser()
    {
        // Arrange
        using using HttpRequestMessage httpRequest = HttpClientExtensions.CreateDeleteMessage($"/api/account/me/addresses/{apiFixture.ApiFactory.DataSeeder.AnotherUserAddressId}");
        httpRequest.AddIdentityAuthorizationHeader(apiFixture.ApiFactory.DataSeeder.SeededUserKeycloakId);


        // Act
        HttpResponseMessage response = await apiFixture.ApiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAddress_ShouldReturn401_WhenUnauthenticated()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateDeleteMessage($"/api/account/me/addresses/{apiFixture.ApiFactory.DataSeeder.SeededAddressId}");

        // Act
        HttpResponseMessage response = await apiFixture.ApiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
