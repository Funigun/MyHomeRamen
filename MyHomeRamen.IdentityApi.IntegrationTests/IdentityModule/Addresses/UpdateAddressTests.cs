using System.Net;
using MyHomeRamen.Common.Contracts.Users.Account.Requests;
using MyHomeRamen.Common.Contracts.Users.Account.Responses;
using MyHomeRamen.IdentityApi.IntegrationTests.Common;
using MyHomeRamen.IdentityApi.IntegrationTests.Common.Configuration;
using MyHomeRamen.IdentityApi.IntegrationTests.Common.Data;

namespace MyHomeRamen.IdentityApi.IntegrationTests.IdentityModule.Addresses;

public sealed class UpdateAddressTests(IdentityApiFixture apiFixture) : IClassFixture<IdentityApiFixture>
{
    [Fact]
    public async Task UpdateAddress_ShouldReturn200_WithUpdatedAddress()
    {
        // Arrange
        UpdateAddressRequest request = DataGenerator.GenerateValidUpdateAddressRequest(isDefault: false);

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePutMessage($"/api/account/me/addresses/{apiFixture.ApiFactory.DataSeeder.SeededAddressId}");
        httpRequest.WithJsonContent(request);
        httpRequest.AddIdentityAuthorizationHeader(apiFixture.ApiFactory.DataSeeder.SeededUserKeycloakId);

        // Act
        HttpResponseMessage response = await apiFixture.ApiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.OK);

        UpdateAddressResponse? body = await response.ResponseToDto<UpdateAddressResponse>();
        Assert.NotNull(body);
        Assert.Equal(apiFixture.ApiFactory.DataSeeder.SeededAddressId, body.Id);
    }

    [Fact]
    public async Task UpdateAddress_ShouldReturn400_WhenAddressNotFound()
    {
        // Arrange
        UpdateAddressRequest request = DataGenerator.GenerateValidUpdateAddressRequest();

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePutMessage($"/api/account/me/addresses/{Guid.NewGuid()}");
        httpRequest.WithJsonContent(request);
        httpRequest.AddIdentityAuthorizationHeader(apiFixture.ApiFactory.DataSeeder.SeededUserKeycloakId);

        // Act
        HttpResponseMessage response = await apiFixture.ApiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateAddress_ShouldReturn401_WhenUnauthenticated()
    {
        // Arrange
        UpdateAddressRequest request = DataGenerator.GenerateValidUpdateAddressRequest();

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePutMessage($"/api/account/me/addresses/{apiFixture.ApiFactory.DataSeeder.SeededAddressId}");
        httpRequest.WithJsonContent(request);

        // Act
        HttpResponseMessage response = await apiFixture.ApiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.Unauthorized);
    }

    [Theory]
    [MemberData(nameof(DataGenerator.InvalidUpdateAddressRequests), MemberType = typeof(DataGenerator))]
    public async Task UpdateAddress_ShouldReturn400_WhenPayloadInvalid(UpdateAddressRequest request)
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePutMessage($"/api/account/me/addresses/{apiFixture.ApiFactory.DataSeeder.SeededAddressId}");
        httpRequest.WithJsonContent(request);
        httpRequest.AddIdentityAuthorizationHeader(apiFixture.ApiFactory.DataSeeder.SeededUserKeycloakId);

        // Act
        HttpResponseMessage response = await apiFixture.ApiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }
}
