using MyHomeRamen.Api.Common.Endpoint.Models;

namespace MyHomeRamen.Api.Users.Features.Account.CreateAddress.Models;

public sealed record AddAddressRequest(
    string Street,
    string Building,
    string? Apartment,
    string City,
    string ZipCode,
    bool IsDefault) : IRequest<Guid>;
