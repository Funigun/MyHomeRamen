using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.ShoppingCart.BasketItems;

namespace MyHomeRamen.Persistance.ShoppingCart.Converters;

public class BasketItemIdConverter : ValueConverter<BasketItemId, Guid>
{
    public BasketItemIdConverter() : base(id => id.Value, value => new BasketItemId(value))
    {
    }
}
