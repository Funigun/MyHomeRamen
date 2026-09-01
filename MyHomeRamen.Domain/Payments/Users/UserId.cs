using MyHomeRamen.Domain.Abstractions;

namespace MyHomeRamen.Domain.Payments.Users;

public readonly record struct UserId(Guid Value) : IEntityId
{
    public static implicit operator Guid(UserId id) => id.Value;
    public static implicit operator UserId(Guid value) => new(value);

    public override readonly string ToString() => Value.ToString();
}
