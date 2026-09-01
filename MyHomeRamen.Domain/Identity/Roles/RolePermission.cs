using MyHomeRamen.Domain.Abstractions;
using MyHomeRamen.Domain.Identity.Permissions;

namespace MyHomeRamen.Domain.Identity.Roles;

public sealed class RolePermission : IEntity<RolePermissionId>
{
    public RolePermissionId Id { get; private set; } = default!;

    public RoleId RoleId { get; private set; } = default!;

    public PermissionId PermissionId { get; private set; } = default!;

    private RolePermission() { }

    public static RolePermission Create(RoleId roleId, PermissionId permissionId)
    {
        return new()
        {
            Id = new RolePermissionId(Guid.CreateVersion7()),
            RoleId = roleId,
            PermissionId = permissionId
        };
    }
}
