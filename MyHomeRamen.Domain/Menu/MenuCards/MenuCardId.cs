using MyHomeRamen.Domain.Abstractions;

namespace MyHomeRamen.Domain.Menu.MenuCards;

public readonly record struct MenuCardId(Guid Value) : IEntityId
{
    public static implicit operator Guid(MenuCardId id) => id.Value;

    public static implicit operator MenuCardId(Guid value) => new(value);

    public override readonly string ToString() => Value.ToString();
}
