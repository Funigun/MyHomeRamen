using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.Restaurants.Restaurants;

namespace MyHomeRamen.Persistance.Restaurants.Converters;

public class RestaurantIdConverter : ValueConverter<RestaurantId, Guid>
{
    public RestaurantIdConverter() : base(id => id.Value, value => new RestaurantId(value))
    {
    }
}
