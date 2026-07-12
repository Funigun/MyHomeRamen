using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Common.Contracts.Users.Account.Responses;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.Features.Identity.Abstractions;

namespace MyHomeRamen.Features.Identity.Features.Users.GetAddresses;

public sealed class GetAddressesHandler(IIdentityDbContext dbContext, ICurrentUser currentUser) : IQueryHandler<GetAddressesQuery, GetAddressesResponse>
{
    public async Task<GetAddressesResponse> Handle(GetAddressesQuery query, CancellationToken cancellationToken)
    {
        User? user = await dbContext.User.Query().ById(new UserId(currentUser.UserId), cancellationToken);

        IEnumerable<AddressDto> addresses = user?.Addresses.Select(a => a.ToDto()) ?? [];

        return new GetAddressesResponse(addresses);
    }
}

