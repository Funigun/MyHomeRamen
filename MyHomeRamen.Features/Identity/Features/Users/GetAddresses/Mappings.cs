using MyHomeRamen.Common.Contracts.Users.Account.Responses;
using MyHomeRamen.Domain.Identity.Users;

namespace MyHomeRamen.Features.Identity.Features.Users.GetAddresses;

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

