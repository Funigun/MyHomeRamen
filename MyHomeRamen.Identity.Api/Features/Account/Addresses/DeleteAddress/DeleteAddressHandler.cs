using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Authorization;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Domain.Users;
using MyHomeRamen.Domain.Users.Database;
using MyHomeRamen.Identity.Api.Features.Account.Addresses.DeleteAddress.Models;

namespace MyHomeRamen.Identity.Api.Features.Account.Addresses.DeleteAddress;

public sealed class DeleteAddressHandler(IUsersDbContext dbContext, ICurrentUser currentUser) : IRequestHandler<DeleteAddressRequest, IResult>
{
    public async Task<IResult> Handle([FromRoute] DeleteAddressRequest id, CancellationToken cancellationToken)
    {
        User user = await dbContext.Users.Include(u => u.Addresses)
                                         .FirstAsync(u => u.Id == currentUser.UserId, cancellationToken);

        user.RemoveAddress(id.Id);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }
}
