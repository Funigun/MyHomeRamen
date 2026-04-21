using MyHomeRamen.Domain.Common.Address;

namespace MyHomeRamen.Domain.Users;

internal static class AddressValidator
{
    internal static void ValidateAddress(Address address)
    {
        if (string.IsNullOrWhiteSpace(address.Street))
        {
            throw AddressErrors.StreetRequired();
        }

        if (address.Street.Length > AddressConstants.MaxStreetLength)
        {
            throw AddressErrors.StreetTooLong();
        }

        if (string.IsNullOrWhiteSpace(address.Building))
        {
            throw AddressErrors.BuildingRequired();
        }

        if (address.Building.Length > AddressConstants.MaxBuildingLength)
        {
            throw AddressErrors.BuildingTooLong();
        }

        if (!string.IsNullOrEmpty(address.Apartment) && address.Apartment.Length > AddressConstants.MaxApartmentLength)
        {
            throw AddressErrors.ApartmentTooLong();
        }

        if (string.IsNullOrWhiteSpace(address.City))
        {
            throw AddressErrors.CityRequired();
        }

        if (address.City.Length > AddressConstants.MaxCityLength)
        {
            throw AddressErrors.CityTooLong();
        }

        if (string.IsNullOrWhiteSpace(address.ZipCode))
        {
            throw AddressErrors.ZipCodeRequired();
        }

        if (address.ZipCode.Length > AddressConstants.MaxZipCodeLength)
        {
            throw AddressErrors.ZipCodeTooLong();
        }
    }
}
