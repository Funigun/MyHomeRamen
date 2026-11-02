using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.Menu.MenuCards;

namespace MyHomeRamen.Persistance.Menu.Converters;

public sealed class MenuCardIdConverter : ValueConverter<MenuCardId, Guid>
{
    public MenuCardIdConverter()
        : base(id => id.Value, value => new MenuCardId(value))
    {
    }
}
