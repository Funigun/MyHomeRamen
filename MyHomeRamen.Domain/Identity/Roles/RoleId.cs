using MyHomeRamen.Domain.Abstractions;

namespace MyHomeRamen.Domain.Identity.Roles;

public readonly record struct RoleId(Guid Value) : IEntityId
{
    public static implicit operator Guid(RoleId id) => id.Value;

    public static implicit operator RoleId(Guid value) => new(value);

    public override readonly string ToString() => Value.ToString();
}
