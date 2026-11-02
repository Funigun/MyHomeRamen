using MyHomeRamen.Domain.Abstractions;

namespace MyHomeRamen.Domain.Menu.MenuSections;

public readonly record struct MenuSectionId(Guid Value) : IEntityId
{
    public static implicit operator Guid(MenuSectionId id) => id.Value;

    public static implicit operator MenuSectionId(Guid value) => new(value);

    public override readonly string ToString() => Value.ToString();
}
