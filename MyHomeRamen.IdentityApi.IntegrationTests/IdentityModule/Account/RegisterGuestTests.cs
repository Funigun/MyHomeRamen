using System.Net;
using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.Features.Identity.Features.Users.RegisterGuest;
using MyHomeRamen.Features.Identity.Permissions;
using MyHomeRamen.IdentityApi.IntegrationTests.Common;
using MyHomeRamen.IntegrationTests.Extensions;

namespace MyHomeRamen.IdentityApi.IntegrationTests.IdentityModule.Account;

public sealed class RegisterGuestTests(IdentityWebApiFactory apiFactory) : IClassFixture<IdentityWebApiFactory>
{
    [Fact]
    public async Task RegisterGuest_ShouldAssignGuestRole_WhenGuestIsCreated()
    {
        // Arrange
        using HttpRequestMessage request = HttpClientExtensions.CreatePostMessage("/api/account/guest");

        // Act
        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        await response.AssertStatusCode(HttpStatusCode.Created);
        RegisterGuestResponse body = await response.ResponseToDto<RegisterGuestResponse>();

        User? guest = await apiFactory.IdentityDbContext.User.Query()
            .ByGuestId(body.GuestId, TestContext.Current.CancellationToken);

        Assert.NotNull(guest);

        IReadOnlyCollection<string> permissions = (await apiFactory.IdentityDbContext.Permission.Query()
                .ByUserId(guest!.Id.Value, TestContext.Current.CancellationToken))
            .Select(permission => permission.Name)
            .ToArray();

        Assert.Contains(ShoppingCartPermissionConstants.CanViewBasket, permissions);
        Assert.Contains(ShoppingCartPermissionConstants.CanAddProduct, permissions);
        Assert.Contains(ShoppingCartPermissionConstants.CanCheckout, permissions);
    }
}
