namespace MyHomeRamen.Blazor.Features.Account.Common.Models;

public record UpdateAddressRequest(string Street, string Building, string? Apartment, string City, string ZipCode, bool IsDefault);
