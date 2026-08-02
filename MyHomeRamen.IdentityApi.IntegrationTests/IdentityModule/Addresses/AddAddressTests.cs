using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Common.Contracts.Users.Account.Requests;
using MyHomeRamen.Common.Contracts.Users.Account.Responses;
using MyHomeRamen.IdentityApi.IntegrationTests.Common;
using MyHomeRamen.IdentityApi.IntegrationTests.Common.Configuration;
using MyHomeRamen.IdentityApi.IntegrationTests.Common.Data;

namespace MyHomeRamen.IdentityApi.IntegrationTests.IdentityModule.Addresses;

public sealed class AddAddressTests(IdentityApiFixture apiFixture) : IClassFixture<IdentityApiFixture>
{
    [Fact]
    public async Task AddAddress_ShouldReturn201_WithNewAddress()
    {
        // Arrange
        HttpStatusCode expectedStatusCode = HttpStatusCode.Created;

        CreateAddressRequest request = DataGenerator.GenerateValidAddAddressRequest(isDefault: false);

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage("/api/account/me/addresses");
        httpRequest.WithJsonContent(request);
        httpRequest.AddIdentityAuthorizationHeader(apiFixture.ApiFactory.DataSeeder.SeededUserKeycloakId);

        // Act
        HttpResponseMessage response = await apiFixture.ApiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(expectedStatusCode);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        CreateAddressResponse? body = await response.Content.ReadFromJsonAsync<CreateAddressResponse>(TestContext.Current.CancellationToken);
        Assert.NotNull(body);
        Assert.NotEqual(Guid.Empty, body.Id);
    }

    [Fact]
    public async Task AddAddress_ShouldReturn400_WhenUserHas5Addresses()
    {
        // Arrange
        CreateAddressRequest request = DataGenerator.GenerateValidAddAddressRequest();

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage("/api/account/me/addresses");
        httpRequest.WithJsonContent(request);
        httpRequest.AddIdentityAuthorizationHeader(apiFixture.ApiFactory.DataSeeder.FullAddressesUserKeycloakId);

        // Act
        HttpResponseMessage response = await apiFixture.ApiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddAddress_ShouldReturn401_WhenUnauthenticated()
    {
        // Arrange
        CreateAddressRequest request = DataGenerator.GenerateValidAddAddressRequest();

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage("/api/account/me/addresses");
        httpRequest.WithJsonContent(request);

        // Act
        HttpResponseMessage response = await apiFixture.ApiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.Unauthorized);
    }

    [Theory]
    [MemberData(nameof(DataGenerator.InvalidAddAddressRequests), MemberType = typeof(DataGenerator))]
    public async Task AddAddress_ShouldReturn400_WhenPayloadInvalid(CreateAddressRequest request)
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage("/api/account/me/addresses");
        httpRequest.WithJsonContent(request);
        httpRequest.AddIdentityAuthorizationHeader(apiFixture.ApiFactory.DataSeeder.SeededUserKeycloakId);

        // Act
        HttpResponseMessage response = await apiFixture.ApiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }
}
