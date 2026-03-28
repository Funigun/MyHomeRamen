using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.ShoppingCart.Users;

public readonly record struct RoleId(Guid Value) : IEntityId
{
    public static implicit operator Guid(RoleId id) => id.Value;

    public static implicit operator RoleId(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
