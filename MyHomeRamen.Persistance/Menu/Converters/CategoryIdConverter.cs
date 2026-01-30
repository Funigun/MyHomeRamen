using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.Menu;

namespace MyHomeRamen.Persistance.Menu.Converters;

public class CategoryIdConverter : ValueConverter<CategoryId, Guid>
{
    public CategoryIdConverter() : base(id => id.Value, value => new CategoryId(value))
    {
    }
}
