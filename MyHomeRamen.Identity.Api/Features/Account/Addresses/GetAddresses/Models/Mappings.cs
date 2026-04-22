using MyHomeRamen.Domain.Users;

namespace MyHomeRamen.Identity.Api.Features.Account.Addresses.GetAddresses.Models;

internal static class Mappings
{
    extension(Address address)
    {
        internal AddressDto ToDto()
        {
            return new AddressDto(
                address.Id,
                address.Street,
                address.Building,
                address.Apartment,
                address.City,
                address.ZipCode,
                address.IsDefault);
        }
    }
}
