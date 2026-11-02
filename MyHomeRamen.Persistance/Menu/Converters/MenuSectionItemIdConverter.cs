using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.Menu.MenuSectionItems;

namespace MyHomeRamen.Persistance.Menu.Converters;

public sealed class MenuSectionItemIdConverter : ValueConverter<MenuSectionItemId, Guid>
{
    public MenuSectionItemIdConverter()
        : base(id => id.Value, value => new MenuSectionItemId(value))
    {
    }
}
