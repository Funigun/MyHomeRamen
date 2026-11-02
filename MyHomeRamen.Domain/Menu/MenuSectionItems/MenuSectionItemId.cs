using MyHomeRamen.Domain.Abstractions;

namespace MyHomeRamen.Domain.Menu.MenuSectionItems;

public readonly record struct MenuSectionItemId(Guid Value) : IEntityId
{
    public static implicit operator Guid(MenuSectionItemId id) => id.Value;

    public static implicit operator MenuSectionItemId(Guid value) => new(value);

    public override readonly string ToString() => Value.ToString();
}
