using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Authorization;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Domain.Users;
using MyHomeRamen.Domain.Users.Database;
using MyHomeRamen.Identity.Api.Features.Account.Addresses.UpdateAddress.Models;

namespace MyHomeRamen.Identity.Api.Features.Account.Addresses.UpdateAddress;

public sealed class UpdateAddressHandler(IUsersDbContext dbContext, ICurrentUser currentUser) : IRequestHandler<UpdateAddressRequest, UpdateAddressResponse>
{
    public async Task<UpdateAddressResponse> Handle(UpdateAddressRequest request, CancellationToken cancellationToken)
    {
        User? user = await dbContext.Users
            .Include(u => u.Addresses)
            .FirstOrDefaultAsync(u => u.KeycloakUserId == currentUser.Id, cancellationToken);

        user!.UpdateAddress(request.Id, request.Street, request.Building, request.Apartment ?? string.Empty, request.City, request.ZipCode, request.IsDefault);

        await dbContext.SaveChangesAsync(cancellationToken);

        Address address = user.Addresses.First(a => a.Id == request.Id);

        return new(address.Id);
    }
}
