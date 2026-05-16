using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Authorization;
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Domain.Users;
using MyHomeRamen.Domain.Users.Database;

namespace MyHomeRamen.Api.Users.Features.Account.DeleteAddress;

public sealed class DeleteAddressHandler(IUsersDbContext dbContext, ICurrentUser currentUser) : ICommandHandler<DeleteAddressCommand>
{
    public async Task Handle(DeleteAddressCommand command, CancellationToken cancellationToken)
    {
        User user = await dbContext.Users.Include(u => u.Addresses)
                                         .FirstAsync(u => u.Id == currentUser.UserId, cancellationToken);

        user.RemoveAddress(command.Id);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
