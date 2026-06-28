using MyHomeRamen.Common.Contracts.Users.Account.Responses;
using MyHomeRamen.Domain.Users;

namespace MyHomeRamen.Features.Users.Features.Account.GetAddresses;

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

