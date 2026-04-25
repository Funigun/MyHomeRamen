namespace MyHomeRamen.Blazor.Features.Account.Common.Models;

public record GetAddressesResponse(IEnumerable<AddressDto> Addresses);

public record AddressDto(Guid Id, string Street, string Building, string? Apartment, string City, string ZipCode, bool IsDefault);
