using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.Menu.Offers;

namespace MyHomeRamen.Persistance.Menu.Converters;

public sealed class RestaurantProductOfferIdConverter : ValueConverter<RestaurantProductOfferId, Guid>
{
    public RestaurantProductOfferIdConverter()
        : base(id => id.Value, value => new RestaurantProductOfferId(value))
    {
    }
}
