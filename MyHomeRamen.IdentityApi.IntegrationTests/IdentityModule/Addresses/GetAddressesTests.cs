using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using MyHomeRamen.Domain.Identity.Roles;
using MyHomeRamen.Features.Identity.Features.Users.GetAddresses;
using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.IdentityApi.IntegrationTests.Common;
using MyHomeRamen.IntegrationTests.Extensions;
using MyHomeRamen.Features.Identity.Abstractions;

namespace MyHomeRamen.IdentityApi.IntegrationTests.IdentityModule.Addresses;

public sealed class GetAddressesTests(IdentityWebApiFactory apiFactory) : IClassFixture<IdentityWebApiFactory>
{
    private const string Endpoint = "/api/account/me/addresses";

    [Fact]
    public async Task GetAddresses_ShouldReturn200_WithAddressList()
    {
        // Arrange
        Address defaultAddress = Address.Create("123 Main St", "Building A", "Apt 1", "Cityville", "12345", isDefault: true);
        Address address = Address.Create("123 Main St", "Building A", "Apt 1", "Cityville", "12345", isDefault: false);

        using IServiceScope scope = apiFactory.CreateSeedScope();
        IIdentityDbContext dbContext = scope.ServiceProvider.GetRequiredService<IIdentityDbContext>();
        User user = await dbContext.User.Load().ById(apiFactory.IdentityTestData.EmployeeUser.UserId, TestContext.Current.CancellationToken);
        user.AddAddress(defaultAddress);
        user.AddAddress(address);

        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage(Endpoint);
        httpRequest.AddAuthorizationHeader(apiFactory.IdentityTestData.EmployeeUser);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

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
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAddresses_ShouldReturnEmptyList_WhenUserHasNoAddresses()
    {
        // Arrange
        
        Role role = await apiFactory.IdentityDbContext.Role.Load().ByName("Customer", TestContext.Current.CancellationToken);

        User newUser = User.Create(
            keycloakUserId: "test-no-addresses-user",
            userName: "noaddressesuser",
            firstName: "No",
            lastName: "Addresses",
            email: "noaddresses@example.com",
            phoneNumber: "000000000",
            role: role);

        apiFactory.IdentityDbContext.User.Add(newUser);
        await apiFactory.IdentityDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreateGetMessage(Endpoint);
        httpRequest.AddAuthorizationHeader(apiFactory.IdentityTestData.CustomerUser);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        GetAddressesResponse? body = await response.Content.ReadFromJsonAsync<GetAddressesResponse>(TestContext.Current.CancellationToken);
        Assert.NotNull(body);
        Assert.Empty(body.Addresses);
    }
}
