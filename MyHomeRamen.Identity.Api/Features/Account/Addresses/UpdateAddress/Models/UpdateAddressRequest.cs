using MyHomeRamen.Api.Common.Endpoint.Models;

namespace MyHomeRamen.Identity.Api.Features.Account.Addresses.UpdateAddress.Models;

public sealed record UpdateAddressRequest(
    Guid Id,
    string Street,
    string Building,
    string? Apartment,
    string City,
    string ZipCode,
    bool IsDefault) : IRequest<UpdateAddressResponse>;
