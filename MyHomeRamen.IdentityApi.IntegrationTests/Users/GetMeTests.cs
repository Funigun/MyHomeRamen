using System.Net;
using MyHomeRamen.Features.Identity.Features.Users.GetId;
using MyHomeRamen.Features.Identity.Permissions;
using MyHomeRamen.IdentityApi.IntegrationTests.Common;
using MyHomeRamen.IntegrationTests.Extensions;

namespace MyHomeRamen.IdentityApi.IntegrationTests.Users;

public sealed class GetMeTests(WebApiFactory apiFactory) : IClassFixture<WebApiFactory>
{
    private const string Endpoint = "/api/identity/users/me";

    [Fact]
    public async Task GetMe_ShouldReturnUserAndPanelActions_ForAssignedPermissions()
    {
        // Arrange
        (string KeycloakId, Guid UserId) user = await apiFactory.IdentityTestData.SeedUser(
            ("Panel User", [RestaurantsPermissionConstants.RestaurantManage, RestaurantsPermissionConstants.CompanyView]),
            "panel-user",
            "Alex");

        using HttpRequestMessage request = HttpClientExtensions.CreateGetMessage(Endpoint);
        request.AddAuthorizationHeader(user);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.OK);
        GetMeResponse body = await response.ResponseToDto<GetMeResponse>();
        Assert.Equal(user.UserId, body.UserId);
        Assert.Equal("Alex", body.FirstName);
        Assert.Equal(new AdminActions(CanViewPanel: true), body.AdminActions);
        Assert.Equal(new OwnerActions(CanViewPanel: true), body.OwnerActions);
    }

    [Fact]
    public async Task GetMe_ShouldReturnNullProfileAndActions_ForGuest()
    {
        // Arrange
        (Guid UserId, Guid GuestId) guest = await apiFactory.IdentityTestData.SeedGuest([]);
        using HttpRequestMessage request = HttpClientExtensions.CreateGetMessage(Endpoint)
            .WithGuestCookie(guest.GuestId.ToString());

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.OK);
        GetMeResponse body = await response.ResponseToDto<GetMeResponse>();
        Assert.Equal(guest.UserId, body.UserId);
        Assert.Null(body.FirstName);
        Assert.Null(body.AdminActions);
        Assert.Null(body.OwnerActions);
    }

    [Fact]
    public async Task GetMe_ShouldReturn403_WhenNoCurrentDomainUserExists()
    {
        // Arrange
        using HttpRequestMessage request = HttpClientExtensions.CreateGetMessage(Endpoint);

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.Forbidden);
    }
}
