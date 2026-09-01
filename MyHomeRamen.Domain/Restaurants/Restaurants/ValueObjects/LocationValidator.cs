using MyHomeRamen.Domain.Common.Restaurant;

namespace MyHomeRamen.Domain.Restaurants.Restaurants.ValueObjects;

internal static class LocationValidator
{
    internal static void Validate(Location location)
    {
        if (location.Latitude is < -90 or > 90)
        {
            throw RestaurantErrors.InvalidLatitude();
        }

        if (location.Longitude is < -180 or > 180)
        {
            throw RestaurantErrors.InvalidLongitude();
        }
    }
}
