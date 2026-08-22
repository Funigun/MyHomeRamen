using MyHomeRamen.Domain.Abstractions;

namespace MyHomeRamen.Domain.Identity.Roles;

public readonly record struct RolePermissionId(Guid Value) : IEntityId
{
    public static implicit operator Guid(RolePermissionId id) => id.Value;

    public static implicit operator RolePermissionId(Guid value) => new(value);

    public override readonly string ToString() => Value.ToString();
}
