using MyHomeRamen.Api.Common.Authorization;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Common.Contracts.Users.Account.Responses;
using MyHomeRamen.Domain.Users;
using MyHomeRamen.Domain.Users.Database;
using MyHomeRamen.Persistance.Users.Extensions;

namespace MyHomeRamen.Api.Users.Features.Account.CreateAddress;

public sealed class CreateAddressHandler(IUsersDbContext dbContext, ICurrentUser currentUser) : IRequestHandler<CreateAddressCommand, CreateAddressResponse>
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
