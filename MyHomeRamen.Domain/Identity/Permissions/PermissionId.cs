using MyHomeRamen.Domain.Abstractions;

namespace MyHomeRamen.Domain.Identity.Permissions;

public record struct PermissionId(Guid Value) : IEntityId
{
    public static implicit operator PermissionId(Guid value) => new(value);

    public override readonly string ToString() => Value.ToString();
}
