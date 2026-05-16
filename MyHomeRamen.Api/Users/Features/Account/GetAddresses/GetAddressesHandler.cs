using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Authorization;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Common.Contracts.Users.Account.Responses;
using MyHomeRamen.Domain.Users;
using MyHomeRamen.Domain.Users.Database;

namespace MyHomeRamen.Api.Users.Features.Account.GetAddresses;

public sealed class GetAddressesHandler(IUsersDbContext dbContext, ICurrentUser currentUser) : IRequestHandler<GetAddressesQuery, GetAddressesResponse>
{
    public async Task<GetAddressesResponse> Handle(GetAddressesQuery query, CancellationToken cancellationToken)
    {
        User? user = await dbContext.Users.AsNoTracking()
                                          .Include(u => u.Addresses)
                                          .FirstOrDefaultAsync(u => u.Id == currentUser.UserId, cancellationToken);

        IEnumerable<AddressDto> addresses = user?.Addresses.Select(a => a.ToDto()) ?? [];

        return new GetAddressesResponse(addresses);
    }
}
