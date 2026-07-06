using MyHomeRamen.Common.Contracts.Messaging;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Permissions;
using MyHomeRamen.Domain.ShoppingCart.Roles;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.ShoppingCart.Features.Abstractions;
using MyHomeRamen.Worker.Common;
using MyHomeRamen.Worker.MessagesHandler.Common;

namespace MyHomeRamen.Worker.MessagesHandler.ShoppingCart;

public class ShoppingCartUserRegisteredHandler(IShoppingCartDbContext dbContext) : IIntegrationEventHandler<UserRegisteredIntegrationEvent>
{
    public async Task HandleAsync(UserRegisteredIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        UserId userId = new(integrationEvent.Id);
        bool userExists = await dbContext.User.Query().FindByIdAsync(userId, cancellationToken) != null;

        if (userExists)
        {
            return;
        }

        string shoppingCartRoleName = MapIdentityProviderRoleToShoppingCartRole(integrationEvent.Role);

        Role? customerRole = await dbContext.Role.Query().GetByNameWithPermissionsAsync(shoppingCartRoleName, cancellationToken);

        List<Role> roles = customerRole != null ? [customerRole] : [];
        List<Permission> permissions = customerRole != null ? customerRole.Permissions.ToList() : [];

        User user = User.Create(userId, roles, permissions);

        Basket basket = Basket.Create(new BasketId(Guid.CreateVersion7()), user);

        dbContext.User.Add(user);
        dbContext.Basket.Add(basket);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static string MapIdentityProviderRoleToShoppingCartRole(string identityProviderRole)
    {
        return identityProviderRole switch
        {
            AuthorizationConstants.EmployeeRole => RoleConstants.Employee,
            AuthorizationConstants.CustomerRole => RoleConstants.Customer,
            _ => throw new InvalidOperationException($"Unsupported role: {identityProviderRole}")
        };
    }
}
