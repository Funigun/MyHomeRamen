using MyHomeRamen.Domain.Abstractions;

namespace MyHomeRamen.Domain.Reservations.Users;

public readonly record struct UserId(Guid Value) : IEntityId
{
    public static implicit operator Guid(UserId id) => id.Value;
    public static implicit operator UserId(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
