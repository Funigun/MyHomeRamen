namespace MyHomeRamen.Api.Users.Features.Account.GetAddresses.Models;

public sealed record GetAddressesResponse(IEnumerable<AddressDto> Addresses);

public sealed record AddressDto(Guid Id, string Street, string Building, string Apartment, string City, string ZipCode, bool IsDefault);
