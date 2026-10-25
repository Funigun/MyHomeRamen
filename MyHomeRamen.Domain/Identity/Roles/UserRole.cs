using MyHomeRamen.Domain.Abstractions;
using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Identity.Users;

namespace MyHomeRamen.Domain.Identity.Roles;

public sealed class UserRole : AuditableEntity, IEntity<UserRoleId>
{
    public UserRoleId Id { get; private set; }

    public UserId UserId { get; private set; }

    public RoleId RoleId { get; private set; }

    public User User { get; private set; } = default!;

    public Role Role { get; private set; } = default!;

    private UserRole() { }

    private UserRole(UserRoleId id, User user, Role role)
    {
        Id = id;
        User = user;
        Role = role;
    }

    public static UserRole Create(User user, Role role)
    {
        UserRole userRole = new(new UserRoleId(Guid.CreateVersion7()), user, role)
        {
            UserId = user.Id,
            RoleId = role.Id
        };

        UserRoleValidator.ValidateUserRole(userRole);
        return userRole;
    }
}
