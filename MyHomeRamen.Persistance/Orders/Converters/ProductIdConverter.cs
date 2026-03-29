using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.Orders.Products;

namespace MyHomeRamen.Persistance.Orders.Converters;

public class ProductIdConverter : ValueConverter<ProductId, Guid>
{
    public ProductIdConverter() : base(id => id.Value, value => new ProductId(value)) { }
}
