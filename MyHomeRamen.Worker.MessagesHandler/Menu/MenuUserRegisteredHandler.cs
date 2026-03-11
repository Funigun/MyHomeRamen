using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Common.Contracts.Messaging;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Domain.Menu.Users;
using MyHomeRamen.Worker.Common;
using MyHomeRamen.Worker.MessagesHandler.Common;

namespace MyHomeRamen.Worker.MessagesHandler.Menu;

public class MenuUserRegisteredHandler(IMenuDbContext dbContext) : IIntegrationEventHandler<UserRegisteredIntegrationEvent>
{
    public async Task HandleAsync(UserRegisteredIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        UserId userId = new(integrationEvent.Id);
        bool userExists = await dbContext.Users.AnyAsync(u => u.Id == userId, cancellationToken);

        if (userExists)
        {
            return;
        }

        string menuRoleName = MapIdentityProviderRoleToMenuRole(integrationEvent.Role);

        Role? customerRole = await dbContext.Roles.Include(r => r.Permissions)
                                                  .FirstOrDefaultAsync(r => r.Name == menuRoleName, cancellationToken);

        List<Role> roles = customerRole != null ? [customerRole] : [];
        List<Permission> permissions = customerRole != null ? customerRole.Permissions.ToList() : [];

        User user = User.Create(userId, roles, permissions);

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static string MapIdentityProviderRoleToMenuRole(string identityProviderRole)
    {
        return identityProviderRole switch
        {
            AuthorizationConstants.EmployeeRole => RoleConstants.Employee,
            AuthorizationConstants.CustomerRole => RoleConstants.Customer,
            _ => throw new InvalidOperationException($"Unsupported role: {identityProviderRole}")
        };
    }
}
