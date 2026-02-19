using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.ShoppingCart.Users;

public readonly record struct PermissionId(Guid Value) : IEntityId
{
    public static implicit operator Guid(PermissionId id) => id.Value;

    public static implicit operator PermissionId(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
