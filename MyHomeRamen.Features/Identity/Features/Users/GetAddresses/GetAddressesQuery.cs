using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.Features.Identity.Abstractions;

namespace MyHomeRamen.Features.Identity.Features.Users.GetAddresses;

public sealed record GetAddressesQuery : IQuery<GetAddressesResponse>;

public sealed class GetAddressesAuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<GetAddressesQuery>
{
    public async Task<bool> Authorize(GetAddressesQuery request, CancellationToken cancellationToken)
    {
        return currentUser.CanViewUserProfile();
    }
}

public sealed class GetAddressesHandler(IIdentityDbContext dbContext, ICurrentUser currentUser) : IQueryHandler<GetAddressesQuery, GetAddressesResponse>
{
    public async Task<GetAddressesResponse> Handle(GetAddressesQuery query, CancellationToken cancellationToken)
    {
        User? user = await dbContext.User.Query().ById(new UserId(currentUser.UserId), cancellationToken);

        IEnumerable<AddressDto> addresses = user?.Addresses.Select(a => a.ToDto()) ?? [];

        return new GetAddressesResponse(addresses);
    }
}

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
