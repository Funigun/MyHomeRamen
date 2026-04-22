namespace MyHomeRamen.Domain.Common.Address;

public static class AddressErrors
{
    public static DomainException MaxAddressesReached()
        => new($"User cannot have more than {AddressConstants.MaxAddressesPerUser} addresses");

    public static DomainException StreetRequired()
        => new("Street is required");

    public static DomainException StreetTooLong()
        => new($"Street cannot be longer than {AddressConstants.MaxStreetLength} characters");

    public static DomainException BuildingRequired()
        => new("Building is required");

    public static DomainException BuildingTooLong()
        => new($"Building cannot be longer than {AddressConstants.MaxBuildingLength} characters");

    public static DomainException ApartmentTooLong()
        => new($"Apartment cannot be longer than {AddressConstants.MaxApartmentLength} characters");

    public static DomainException CityRequired()
        => new("City is required");

    public static DomainException CityTooLong()
        => new($"City cannot be longer than {AddressConstants.MaxCityLength} characters");

    public static DomainException ZipCodeRequired()
        => new("Zip code is required");

    public static DomainException ZipCodeTooLong()
        => new($"Zip code cannot be longer than {AddressConstants.MaxZipCodeLength} characters");

    public static DomainException AddressNotFound()
        => new("Address not found");
}
