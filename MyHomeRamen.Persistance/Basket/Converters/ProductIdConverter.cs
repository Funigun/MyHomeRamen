using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.Basket;

namespace MyHomeRamen.Persistance.Basket.Converters;

public class ProductIdConverter : ValueConverter<ProductId, Guid>
{
    public ProductIdConverter() : base(id => id.Value, value => new ProductId(value)) { }
}
