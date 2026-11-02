using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.Menu.RestaurantIngredientPrices;

namespace MyHomeRamen.Persistance.Menu.Converters;

public sealed class RestaurantIngredientPriceIdConverter : ValueConverter<RestaurantIngredientPriceId, Guid>
{
    public RestaurantIngredientPriceIdConverter()
        : base(id => id.Value, value => new RestaurantIngredientPriceId(value))
    {
    }
}
