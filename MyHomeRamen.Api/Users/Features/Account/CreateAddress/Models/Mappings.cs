using MyHomeRamen.Domain.Users;

namespace MyHomeRamen.Api.Users.Features.Account.CreateAddress.Models;

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
