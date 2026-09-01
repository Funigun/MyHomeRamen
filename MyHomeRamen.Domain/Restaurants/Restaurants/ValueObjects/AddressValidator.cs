using MyHomeRamen.Domain.Common.Restaurant;

namespace MyHomeRamen.Domain.Restaurants.Restaurants.ValueObjects;

internal static class AddressValidator
{
    internal static void Validate(Address address)
    {
        if (string.IsNullOrWhiteSpace(address.Street))
        {
            throw RestaurantErrors.StreetRequired();
        }

        if (address.Street.Length > RestaurantConstants.MaxStreetLength)
        {
            throw RestaurantErrors.StreetTooLong();
        }

        if (string.IsNullOrWhiteSpace(address.City))
        {
            throw RestaurantErrors.CityRequired();
        }

        if (address.City.Length > RestaurantConstants.MaxCityLength)
        {
            throw RestaurantErrors.CityTooLong();
        }

        if (string.IsNullOrWhiteSpace(address.ZipCode))
        {
            throw RestaurantErrors.ZipCodeRequired();
        }

        if (address.ZipCode.Length > RestaurantConstants.MaxZipCodeLength)
        {
            throw RestaurantErrors.ZipCodeTooLong();
        }

        if (address.Location is null)
        {
            throw RestaurantErrors.LocationRequired();
        }
    }
}
