using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Identity.Api.Features.Account.Addresses.AddAddress.Models;
using MyHomeRamen.IdentityApi.IntegrationTests.Common;
using MyHomeRamen.IdentityApi.IntegrationTests.Common.Configuration;
using MyHomeRamen.IdentityApi.IntegrationTests.Common.Data;

namespace MyHomeRamen.IdentityApi.IntegrationTests.IdentityModule.Addresses;

public sealed class AddAddressTests(IdentityWebApiFactory apiFactory)
{
    [Fact]
    public async Task AddAddress_ShouldReturn201_WithNewAddress()
    {
        // Arrange
        AddAddressRequest request = DataGenerator.GenerateValidAddAddressRequest(isDefault: false);

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage("/api/account/me/addresses")
            .WithJsonContent(request)
            .AddIdentityAuthorizationHeader(DataSeeder.SeededUserKeycloakId);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        AddAddressResponse? body = await response.Content.ReadFromJsonAsync<AddAddressResponse>(TestContext.Current.CancellationToken);
        Assert.NotNull(body);
        Assert.NotEqual(Guid.Empty, body.Id);
    }

    [Fact]
    public async Task AddAddress_ShouldReturn400_WhenUserHas5Addresses()
    {
        // Arrange
        AddAddressRequest request = DataGenerator.GenerateValidAddAddressRequest();

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage("/api/account/me/addresses")
            .WithJsonContent(request)
            .AddIdentityAuthorizationHeader(DataSeeder.FullAddressesUserKeycloakId);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AddAddress_ShouldReturn401_WhenUnauthenticated()
    {
        // Arrange
        AddAddressRequest request = DataGenerator.GenerateValidAddAddressRequest();

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage("/api/account/me/addresses")
            .WithJsonContent(request);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [MemberData(nameof(DataGenerator.InvalidAddAddressRequests), MemberType = typeof(DataGenerator))]
    public async Task AddAddress_ShouldReturn400_WhenPayloadInvalid(AddAddressRequest request)
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage("/api/account/me/addresses")
            .WithJsonContent(request)
            .AddIdentityAuthorizationHeader(DataSeeder.SeededUserKeycloakId);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
