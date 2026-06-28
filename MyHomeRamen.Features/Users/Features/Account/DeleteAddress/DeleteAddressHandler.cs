using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Domain.Users;
using MyHomeRamen.Domain.Users.Database;
using MyHomeRamen.Features.Common.Authorization;

namespace MyHomeRamen.Features.Users.Features.Account.DeleteAddress;

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

