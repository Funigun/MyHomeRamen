using MyHomeRamen.Api.Common.Endpoint.Models;

namespace MyHomeRamen.Api.Users.Features.Account.UpdateAddress.Models;

public sealed record UpdateAddressRequest(
    Guid Id,
    string Street,
    string Building,
    string? Apartment,
    string City,
    string ZipCode,
    bool IsDefault) : IRequest<UpdateAddressResponse>;
