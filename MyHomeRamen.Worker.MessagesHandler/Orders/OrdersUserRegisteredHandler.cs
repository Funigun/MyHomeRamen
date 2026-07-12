using MyHomeRamen.Common.Contracts.Messaging;
using MyHomeRamen.Domain.Orders.Permissions;
using MyHomeRamen.Domain.Orders.Roles;
using MyHomeRamen.Domain.Orders.Users;
using MyHomeRamen.Features.Orders.Features.Abstractions;
using MyHomeRamen.Worker.Common;
using MyHomeRamen.Worker.MessagesHandler.Common;

namespace MyHomeRamen.Worker.MessagesHandler.Orders;

public class OrdersUserRegisteredHandler(IOrdersDbContext dbContext) : IIntegrationEventHandler<UserRegisteredIntegrationEvent>
{
    public async Task HandleAsync(UserRegisteredIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        UserId userId = new(integrationEvent.Id);
        bool userExists = await dbContext.User.Exists(u => u.Id == userId, cancellationToken);

        if (userExists)
        {
            return;
        }

        string orderRoleName = MapIdentityProviderRoleToOrderRole(integrationEvent.Role);

        Role? customerRole = await dbContext.Role.Specification().ByName(orderRoleName, cancellationToken);

        List<Role> roles = customerRole != null ? [customerRole] : [];
        List<Permission> permissions = customerRole != null ? customerRole.Permissions.ToList() : [];

        User user = User.Create(
            userId,
            integrationEvent.FirstName,
            integrationEvent.LastName,
            integrationEvent.Email,
            integrationEvent.PhoneNumber,
            roles,
            permissions);

        dbContext.User.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static string MapIdentityProviderRoleToOrderRole(string identityProviderRole)
    {
        return identityProviderRole switch
        {
            AuthorizationConstants.EmployeeRole => RoleConstants.Employee,
            AuthorizationConstants.CustomerRole => RoleConstants.Customer,
            _ => throw new InvalidOperationException($"Unsupported role: {identityProviderRole}")
        };
    }
}
