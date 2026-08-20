using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Domain.Identity.Roles;
using MyHomeRamen.Features.Identity.Features.Users.GetAddresses;
using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.IdentityApi.IntegrationTests.Common;
using MyHomeRamen.IdentityApi.IntegrationTests.Common.Configuration;

namespace MyHomeRamen.IdentityApi.IntegrationTests.IdentityModule.Addresses;

public sealed class GetAddressesTests(IdentityApiFixture apiFixture) : IClassFixture<IdentityApiFixture>
{
    private const string Endpoint = "/api/account/me/addresses";

    [Fact]
    public async Task GetAddresses_ShouldReturn200_WithAddressList()
    {
        // Arrange
        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage(Endpoint);
        httpRequest.AddIdentityAuthorizationHeader(apiFixture.ApiFactory.DataSeeder.SeededUserKeycloakId);

        // Act
        HttpResponseMessage response = await apiFixture.ApiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

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
        HttpResponseMessage response = await apiFixture.ApiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAddresses_ShouldReturnEmptyList_WhenUserHasNoAddresses()
    {
        // Arrange
        Role role = await apiFixture.ApiFactory.IdentityDbContext.Role.Load().ByName("Customer", TestContext.Current.CancellationToken);

        User newUser = User.Create(
            keycloakUserId: "test-no-addresses-user",
            userName: "noaddressesuser",
            firstName: "No",
            lastName: "Addresses",
            email: "noaddresses@example.com",
            phoneNumber: "000000000",
            role: role);

        apiFixture.ApiFactory.IdentityDbContext.User.Add(newUser);
        await apiFixture.ApiFactory.IdentityDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage(Endpoint);
        httpRequest.AddIdentityAuthorizationHeader("test-no-addresses-user");

        // Act
        HttpResponseMessage response = await apiFixture.ApiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        GetAddressesResponse? body = await response.Content.ReadFromJsonAsync<GetAddressesResponse>(TestContext.Current.CancellationToken);
        Assert.NotNull(body);
        Assert.Empty(body.Addresses);
    }
}
