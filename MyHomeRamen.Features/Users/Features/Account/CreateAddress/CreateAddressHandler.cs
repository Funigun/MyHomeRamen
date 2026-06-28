using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Common.Contracts.Users.Account.Responses;
using MyHomeRamen.Domain.Users;
using MyHomeRamen.Domain.Users.Database;
using MyHomeRamen.Features.Users.Extensions;
using MyHomeRamen.Features.Common.Authorization;

namespace MyHomeRamen.Features.Users.Features.Account.CreateAddress;

public sealed class CreateAddressHandler(IUsersDbContext dbContext, ICurrentUser currentUser) : ICommandHandler<CreateAddressCommand, CreateAddressResponse>
{
    public async Task<CreateAddressResponse> Handle(CreateAddressCommand command, CancellationToken cancellationToken)
    {
        User? user = await dbContext.Users.GetById(currentUser.UserId, cancellationToken);

        Address address = command.Request.ToAddress();

        user!.AddAddress(address);

        dbContext.Addresses.Add(address);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateAddressResponse(address.Id);
    }
}

