using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Common.Contracts.Messaging;
using MyHomeRamen.Domain.Payments.Database;
using MyHomeRamen.Domain.Payments.Users;
using MyHomeRamen.Worker.Common;
using MyHomeRamen.Worker.MessagesHandler.Common;

namespace MyHomeRamen.Worker.MessagesHandler.Payments;

public class PaymentsUserRegisteredHandler(IPaymentsDbContext dbContext) : IIntegrationEventHandler<UserRegisteredIntegrationEvent>
{
    public async Task HandleAsync(UserRegisteredIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        UserId userId = new(integrationEvent.Id);
        bool userExists = await dbContext.Users.AnyAsync(u => u.Id == userId, cancellationToken);

        if (userExists)
        {
            return;
        }

        string paymentsRoleName = MapIdentityProviderRoleToPaymentsRole(integrationEvent.Role);

        Role? customerRole = await dbContext.Roles.Include(r => r.Permissions)
                                                  .FirstOrDefaultAsync(r => r.Name == paymentsRoleName, cancellationToken);

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

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static string MapIdentityProviderRoleToPaymentsRole(string identityProviderRole)
    {
        return identityProviderRole switch
        {
            AuthorizationConstants.EmployeeRole => RoleConstants.Employee,
            AuthorizationConstants.CustomerRole => RoleConstants.Customer,
            _ => throw new InvalidOperationException($"Unsupported role: {identityProviderRole}")
        };
    }
}
