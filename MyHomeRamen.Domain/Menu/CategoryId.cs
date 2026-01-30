using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Menu;

public readonly record struct CategoryId(Guid Value) : IEntityId
{
    public static implicit operator Guid(CategoryId id) => id.Value;

    public static implicit operator CategoryId(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
