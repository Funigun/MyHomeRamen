using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Orders;

public readonly record struct CustomerId(Guid Value) : IEntityId
{
    public static implicit operator Guid(CustomerId id) => id.Value;

    public static implicit operator CustomerId(Guid value) => new(value);

    public override readonly string ToString() => Value.ToString();
}
