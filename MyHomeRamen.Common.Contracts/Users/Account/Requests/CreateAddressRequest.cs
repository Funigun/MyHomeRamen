namespace MyHomeRamen.Common.Contracts.Users.Account.Requests;

public sealed record CreateAddressRequest(
    string Street,
    string Building,
    string? Apartment,
    string City,
    string ZipCode,
    bool IsDefault);
