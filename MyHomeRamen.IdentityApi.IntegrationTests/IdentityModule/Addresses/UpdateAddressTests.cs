using System.Net;
using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.Features.Identity.Features.Users.UpdateAddress;
using MyHomeRamen.IdentityApi.IntegrationTests.Common;
using MyHomeRamen.IdentityApi.IntegrationTests.Common.Data;
using MyHomeRamen.IntegrationTests.Extensions;

namespace MyHomeRamen.IdentityApi.IntegrationTests.IdentityModule.Addresses;

public sealed class UpdateAddressTests(IdentityWebApiFactory apiFactory) : IClassFixture<IdentityWebApiFactory>
{
    [Fact]
    public async Task UpdateAddress_ShouldReturn200_WithUpdatedAddress()
    {
        // Arrange
        Address address = Address.Create("123 Main St", "Building A", "Apt 1", "Cityville", "12345", isDefault: false);
        User user = await apiFactory.IdentityDbContext.User.Load().ById(apiFactory.IdentityTestData.CustomerUser.UserId, TestContext.Current.CancellationToken);
        user.AddAddress(address);
        await apiFactory.IdentityDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        UpdateAddressRequest request = DataGenerator.GenerateValidUpdateAddressRequest(isDefault: false);

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePutMessage($"/api/account/me/addresses/{address.Id}");
        httpRequest.WithJsonContent(request);
        httpRequest.AddAuthorizationHeader(apiFactory.IdentityTestData.CustomerUser);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.OK);

        UpdateAddressResponse? body = await response.ResponseToDto<UpdateAddressResponse>();
        Assert.NotNull(body);
        Assert.Equal(address.Id, body.Id);
    }

    [Fact]
    public async Task UpdateAddress_ShouldReturn400_WhenAddressNotFound()
    {
        // Arrange
        UpdateAddressRequest request = DataGenerator.GenerateValidUpdateAddressRequest();

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePutMessage($"/api/account/me/addresses/{Guid.NewGuid()}");
        httpRequest.WithJsonContent(request);
        httpRequest.AddAuthorizationHeader(apiFactory.IdentityTestData.CustomerUser);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateAddress_ShouldReturn401_WhenUnauthenticated()
    {
        // Arrange
        UpdateAddressRequest request = DataGenerator.GenerateValidUpdateAddressRequest();

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePutMessage($"/api/account/me/addresses/{Guid.NewGuid()}");
        httpRequest.WithJsonContent(request);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.Unauthorized);
    }

    [Theory]
    [MemberData(nameof(DataGenerator.InvalidUpdateAddressRequests), MemberType = typeof(DataGenerator))]
    public async Task UpdateAddress_ShouldReturn400_WhenPayloadInvalid(UpdateAddressRequest request)
    {
        // Arrange
        Address address = Address.Create("123 Main St", "Building A", "Apt 1", "Cityville", "12345", isDefault: false);
        User user = await apiFactory.IdentityDbContext.User.Load().ById(apiFactory.IdentityTestData.CustomerUser.UserId, TestContext.Current.CancellationToken);
        
        if (user.Addresses.Count == 0)
        {
            user.AddAddress(address);
            await apiFactory.IdentityDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePutMessage($"/api/account/me/addresses/{address.Id}");
        httpRequest.WithJsonContent(request);
        httpRequest.AddAuthorizationHeader(apiFactory.IdentityTestData.CustomerUser);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }
}
