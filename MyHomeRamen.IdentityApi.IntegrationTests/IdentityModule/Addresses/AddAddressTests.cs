using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.Features.Identity.Features.Users.CreateAddress;
using MyHomeRamen.IdentityApi.IntegrationTests.Common;
using MyHomeRamen.IdentityApi.IntegrationTests.Common.Data;
using MyHomeRamen.IntegrationTests.Extensions;

namespace MyHomeRamen.IdentityApi.IntegrationTests.IdentityModule.Addresses;

public sealed class AddAddressTests(IdentityWebApiFactory apiFactory) : IClassFixture<IdentityWebApiFactory>
{
    [Fact]
    public async Task AddAddress_ShouldReturn201_WithNewAddress()
    {
        // Arrange
        HttpStatusCode expectedStatusCode = HttpStatusCode.Created;

        CreateAddressRequest request = DataGenerator.GenerateValidAddAddressRequest(isDefault: false);

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage("/api/account/me/addresses");
        httpRequest.WithJsonContent(request);
        httpRequest.AddAuthorizationHeader(apiFactory.IdentityTestData.CustomerUser);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

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
        User user = await apiFactory.IdentityDbContext.User.Load().ById(new UserId(apiFactory.IdentityTestData.ManagerUser.UserId), TestContext.Current.CancellationToken);

        for (int i = 0; i < 5; i++)
        {
            user.AddAddress(Address.Create($"Street {i}", $"Building {i}", $"Apartment {i}", $"City {i}", $"ZipCode {i}", isDefault: false));
        }

        await apiFactory.IdentityDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        CreateAddressRequest request = DataGenerator.GenerateValidAddAddressRequest();

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage("/api/account/me/addresses");
        httpRequest.WithJsonContent(request);
        httpRequest.AddAuthorizationHeader(apiFactory.IdentityTestData.ManagerUser);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

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
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

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
        httpRequest.AddAuthorizationHeader(apiFactory.IdentityTestData.CustomerUser);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }
}
