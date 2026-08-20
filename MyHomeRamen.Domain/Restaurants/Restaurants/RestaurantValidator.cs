using MyHomeRamen.Domain.Common.Restaurant;

namespace MyHomeRamen.Domain.Restaurants.Restaurants;

internal static class RestaurantValidator
{
    internal static void Validate(Restaurant restaurant)
    {
        if (string.IsNullOrWhiteSpace(restaurant.Name))
        {
            throw RestaurantErrors.NameRequired();
        }

        if (restaurant.Name.Length > RestaurantConstants.MaxNameLength)
        {
            throw RestaurantErrors.NameTooLong();
        }

        if (restaurant.Address is null)
        {
            throw RestaurantErrors.AddressRequired();
        }
    }
}
