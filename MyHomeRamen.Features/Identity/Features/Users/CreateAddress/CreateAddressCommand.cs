using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.Features.Identity.Abstractions;

namespace MyHomeRamen.Features.Identity.Features.Users.CreateAddress;

public sealed record CreateAddressCommand(CreateAddressRequest Request) : ICommand<CreateAddressResponse>;

public sealed class CreateAddressHandler(IIdentityDbContext dbContext, ICurrentUser currentUser) : ICommandHandler<CreateAddressCommand, CreateAddressResponse>
{
    public async Task<CreateAddressResponse> Handle(CreateAddressCommand command, CancellationToken cancellationToken)
    {
        User? user = await dbContext.User.Specification().ById(currentUser.UserId, cancellationToken);

        Address address = command.Request.ToAddress();

        user!.AddAddress(address);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateAddressResponse(address.Id);
    }
}

