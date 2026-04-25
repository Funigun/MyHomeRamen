using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Authorization;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Domain.Users;
using MyHomeRamen.Domain.Users.Database;
using MyHomeRamen.Identity.Api.Features.Account.Addresses.GetAddresses.Models;

namespace MyHomeRamen.Identity.Api.Features.Account.Addresses.GetAddresses;

public sealed class GetAddressesHandler(IUsersDbContext dbContext, ICurrentUser currentUser) : IRequestHandler<GetAddressesRequest, GetAddressesResponse>
{
    public async Task<GetAddressesResponse> Handle(GetAddressesRequest request, CancellationToken cancellationToken)
    {
        User? user = await dbContext.Users.AsNoTracking()
                                          .Include(u => u.Addresses)
                                          .FirstOrDefaultAsync(u => u.Id == currentUser.UserId, cancellationToken);

        IEnumerable<AddressDto> addresses = user?.Addresses.Select(a => a.ToDto()) ?? [];

        return new GetAddressesResponse(addresses);
    }
}
