using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.Menu;

namespace MyHomeRamen.Persistance.Menu.Converters;

public class ProductIdConverter : ValueConverter<ProductId, Guid>
{
    public ProductIdConverter() : base(id => id.Value, value => new ProductId(value))
    {
    }
}
