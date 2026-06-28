using MyHomeRamen.Common.Contracts.Users.Account.Requests;
using MyHomeRamen.Domain.Users;

namespace MyHomeRamen.Features.Users.Features.Account.CreateAddress;

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

