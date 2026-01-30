using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.Basket;

namespace MyHomeRamen.Persistance.Basket.Converters;

public class ShoppingCartIdConverter : ValueConverter<ShoppingCartId, Guid>
{
    public ShoppingCartIdConverter() : base(id => id.Value, value => new ShoppingCartId(value)) { }
}
