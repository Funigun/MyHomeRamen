using System.Net;
using MyHomeRamen.Common.Contracts.Users.Account.Requests;
using MyHomeRamen.Common.Contracts.Users.Account.Responses;
using MyHomeRamen.IdentityApi.IntegrationTests.Common;
using MyHomeRamen.IdentityApi.IntegrationTests.Common.Configuration;
using MyHomeRamen.IdentityApi.IntegrationTests.Common.Data;

namespace MyHomeRamen.IdentityApi.IntegrationTests.IdentityModule.Addresses;

public sealed class UpdateAddressTests(IdentityWebApiFactory apiFactory)
{
    [Fact]
    public async Task UpdateAddress_ShouldReturn200_WithUpdatedAddress()
    {
        // Arrange
        UpdateAddressRequest request = DataGenerator.GenerateValidUpdateAddressRequest(isDefault: false);

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePutMessage($"/api/account/me/addresses/{DataSeeder.SeededAddressId}")
            .WithJsonContent(request)
            .AddIdentityAuthorizationHeader(DataSeeder.SeededUserKeycloakId);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        UpdateAddressResponse? body = await response.ResponseToDto<UpdateAddressResponse>();
        Assert.NotNull(body);
        Assert.Equal(DataSeeder.SeededAddressId, body.Id);
    }

    [Fact]
    public async Task UpdateAddress_ShouldReturn400_WhenAddressNotFound()
    {
        // Arrange
        UpdateAddressRequest request = DataGenerator.GenerateValidUpdateAddressRequest();

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePutMessage($"/api/account/me/addresses/{Guid.NewGuid()}")
            .WithJsonContent(request)
            .AddIdentityAuthorizationHeader(DataSeeder.SeededUserKeycloakId);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAddress_ShouldReturn401_WhenUnauthenticated()
    {
        // Arrange
        UpdateAddressRequest request = DataGenerator.GenerateValidUpdateAddressRequest();

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePutMessage($"/api/account/me/addresses/{DataSeeder.SeededAddressId}")
            .WithJsonContent(request);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [MemberData(nameof(DataGenerator.InvalidUpdateAddressRequests), MemberType = typeof(DataGenerator))]
    public async Task UpdateAddress_ShouldReturn400_WhenPayloadInvalid(UpdateAddressRequest request)
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePutMessage($"/api/account/me/addresses/{DataSeeder.SeededAddressId}")
            .WithJsonContent(request)
            .AddIdentityAuthorizationHeader(DataSeeder.SeededUserKeycloakId);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
