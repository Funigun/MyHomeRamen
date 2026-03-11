using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Common.Contracts.Messaging;
using MyHomeRamen.Domain.ShoppingCart;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Database;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Worker.Common;
using MyHomeRamen.Worker.MessagesHandler.Common;

namespace MyHomeRamen.Worker.MessagesHandler.ShoppingCart;

public class ShoppingCartUserRegisteredHandler(IShoppingCartDbContext dbContext) : IIntegrationEventHandler<UserRegisteredIntegrationEvent>
{
    public async Task HandleAsync(UserRegisteredIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        UserId userId = new(integrationEvent.Id);
        bool userExists = await dbContext.Users.AnyAsync(u => u.Id == userId, cancellationToken);

        if (userExists)
        {
            return;
        }

        string shoppingCartRoleName = MapIdentityProviderRoleToShoppingCartRole(integrationEvent.Role);

        Role? customerRole = await dbContext.Roles.Include(r => r.Permissions)
                                                  .FirstOrDefaultAsync(r => r.Name == shoppingCartRoleName, cancellationToken);

        List<Role> roles = customerRole != null ? [customerRole] : [];
        List<Permission> permissions = customerRole != null ? customerRole.Permissions.ToList() : [];

        User user = User.Create(userId, roles, permissions);

        Basket basket = Basket.Create(new BasketId(Guid.CreateVersion7()), user);

        dbContext.Users.Add(user);
        dbContext.ShoppingCarts.Add(basket);

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
