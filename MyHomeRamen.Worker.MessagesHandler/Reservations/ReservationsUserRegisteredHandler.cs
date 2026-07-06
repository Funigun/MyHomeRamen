using MyHomeRamen.Common.Contracts.Messaging;
using MyHomeRamen.Domain.Reservations.Permissions;
using MyHomeRamen.Domain.Reservations.Roles;
using MyHomeRamen.Domain.Reservations.Users;
using MyHomeRamen.Features.Reservations.Features.Abstractions;
using MyHomeRamen.Worker.Common;
using MyHomeRamen.Worker.MessagesHandler.Common;

namespace MyHomeRamen.Worker.MessagesHandler.Reservations;

public class ReservationsUserRegisteredHandler(IReservationsDbContext dbContext) : IIntegrationEventHandler<UserRegisteredIntegrationEvent>
{
    public async Task HandleAsync(UserRegisteredIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        UserId userId = new(integrationEvent.Id);
        bool userExists = await dbContext.User.Query().ExistsAsync(userId, cancellationToken);

        if (userExists)
        {
            return;
        }

        string reservationsRoleName = MapIdentityProviderRoleToReservationsRole(integrationEvent.Role);

        Role? customerRole = await dbContext.Role.Query().GetByNameWithPermissionsAsync(reservationsRoleName, cancellationToken);

        List<Role> roles = customerRole != null ? [customerRole] : [];
        List<Permission> permissions = customerRole != null ? customerRole.Permissions.ToList() : [];

        User user = User.Create(
            userId,
            integrationEvent.FirstName,
            integrationEvent.LastName,
            integrationEvent.Email,
            integrationEvent.PhoneNumber,
            [],
            roles,
            permissions);

        dbContext.User.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static string MapIdentityProviderRoleToReservationsRole(string identityProviderRole)
    {
        return identityProviderRole switch
        {
            AuthorizationConstants.EmployeeRole => RoleConstants.Employee,
            AuthorizationConstants.CustomerRole => RoleConstants.Customer,
            _ => throw new InvalidOperationException($"Unsupported role: {identityProviderRole}")
        };
    }
}
