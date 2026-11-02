using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.Menu.MenuSections;

namespace MyHomeRamen.Persistance.Menu.Converters;

public sealed class MenuSectionIdConverter : ValueConverter<MenuSectionId, Guid>
{
    public MenuSectionIdConverter()
        : base(id => id.Value, value => new MenuSectionId(value))
    {
    }
}
