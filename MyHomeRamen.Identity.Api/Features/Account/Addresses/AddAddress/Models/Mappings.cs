using MyHomeRamen.Domain.Users;
using MyHomeRamen.Identity.Api.Features.Account.Addresses.AddAddress.Models;

namespace MyHomeRamen.Identity.Api.Features.Account.Addresses.AddAddress.Models;

internal static class Mappings
{
    extension(AddAddressRequest request)
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
