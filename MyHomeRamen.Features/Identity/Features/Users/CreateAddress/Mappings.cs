using MyHomeRamen.Common.Contracts.Users.Account.Requests;
using MyHomeRamen.Domain.Identity.Users;

namespace MyHomeRamen.Features.Identity.Features.Users.CreateAddress;

internal static class Mappings
{
    extension(CreateAddressRequest request)
    {
        internal Address ToAddress()
        {
            return Address.Create(
                Guid.CreateVersion7(),
                request.Street,
                request.Building,
                request.Apartment ?? string.Empty,
                request.City,
                request.ZipCode,
                request.IsDefault);
        }
    }
}

