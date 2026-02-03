using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.ShoppingCart;

namespace MyHomeRamen.Persistance.ShoppingCart.Converters;

public class ProductIdConverter : ValueConverter<ProductId, Guid>
{
    public ProductIdConverter() : base(id => id.Value, value => new ProductId(value)) { }
}
