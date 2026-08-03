using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.Features.Identity.Abstractions;

namespace MyHomeRamen.Features.Identity.Features.Users.DeleteAddress;

public record DeleteAddressCommand(Guid Id) : ICommand;

public sealed class DeleteAddressHandler(IIdentityDbContext dbContext, ICurrentUser currentUser) : ICommandHandler<DeleteAddressCommand>
{
    public async Task Handle(DeleteAddressCommand command, CancellationToken cancellationToken)
    {
        User user = await dbContext.User.Specification().ById(new UserId(currentUser.UserId), cancellationToken);

        user.RemoveAddress(command.Id);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

