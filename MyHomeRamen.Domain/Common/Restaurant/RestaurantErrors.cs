namespace MyHomeRamen.Domain.Common.Restaurant;

public static class RestaurantErrors
{
    public static DomainException NameRequired() => new("Restaurant name is required");
    public static DomainException NameTooLong() => new($"Restaurant name cannot be longer than {RestaurantConstants.MaxNameLength} characters");
    public static DomainException AddressRequired() => new("Restaurant address is required");
    public static DomainException ContactDetailsRequired() => new("Restaurant contact details are required");
    public static DomainException BankAccountRequired() => new("Restaurant bank account is required");
    public static DomainException StreetRequired() => new("Restaurant street is required");
    public static DomainException StreetTooLong() => new($"Restaurant street cannot be longer than {RestaurantConstants.MaxStreetLength} characters");
    public static DomainException CityRequired() => new("Restaurant city is required");
    public static DomainException CityTooLong() => new($"Restaurant city cannot be longer than {RestaurantConstants.MaxCityLength} characters");
    public static DomainException ZipCodeRequired() => new("Restaurant zip code is required");
    public static DomainException ZipCodeTooLong() => new($"Restaurant zip code cannot be longer than {RestaurantConstants.MaxZipCodeLength} characters");
    public static DomainException LocationRequired() => new("Restaurant location is required");
    public static DomainException InvalidLatitude() => new("Restaurant latitude must be between -90 and 90");
    public static DomainException InvalidLongitude() => new("Restaurant longitude must be between -180 and 180");
    public static DomainException PhoneRequired() => new("Restaurant phone is required");
    public static DomainException PhoneTooLong() => new($"Restaurant phone cannot be longer than {RestaurantConstants.MaxPhoneLength} characters");
    public static DomainException EmailRequired() => new("Restaurant email is required");
    public static DomainException EmailTooLong() => new($"Restaurant email cannot be longer than {RestaurantConstants.MaxEmailLength} characters");
    public static DomainException AccountNumberRequired() => new("Restaurant account number is required");
    public static DomainException AccountNumberTooLong() => new($"Restaurant account number cannot be longer than {RestaurantConstants.MaxAccountNumberLength} characters");
    public static DomainException BankNameRequired() => new("Restaurant bank name is required");
    public static DomainException BankNameTooLong() => new($"Restaurant bank name cannot be longer than {RestaurantConstants.MaxBankNameLength} characters");
    public static DomainException RoutingNumberRequired() => new("Restaurant routing number is required");
    public static DomainException RoutingNumberTooLong() => new($"Restaurant routing number cannot be longer than {RestaurantConstants.MaxRoutingNumberLength} characters");
    public static DomainException WorkingHoursDayRequired() => new("Working hours day is required");
    public static DomainException WorkingHoursCloseTimeBeforeOpenTime() => new("Working hours close time cannot be before open time");
}
