using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.ShoppingCart.Baskets;

namespace MyHomeRamen.Persistance.ShoppingCart.Converters;

public class BasketIdConverter : ValueConverter<BasketId, Guid>
{
    public BasketIdConverter() : base(id => id.Value, value => new BasketId(value)) { }
}
