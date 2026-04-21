using MyHomeRamen.Api.Common.Endpoint.Models;

namespace MyHomeRamen.Identity.Api.Features.Account.Addresses.AddAddress.Models;

public sealed record AddAddressRequest(
    string Street,
    string Building,
    string? Apartment,
    string City,
    string ZipCode,
    bool IsDefault) : IRequest<Guid>;
