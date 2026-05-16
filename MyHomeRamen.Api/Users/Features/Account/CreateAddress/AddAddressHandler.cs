using MyHomeRamen.Api.Common.Authorization;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Users.Features.Account.CreateAddress.Models;
using MyHomeRamen.Domain.Users;
using MyHomeRamen.Domain.Users.Database;
using MyHomeRamen.Persistance.Users.Extensions;

namespace MyHomeRamen.Api.Users.Features.Account.CreateAddress;

public sealed class AddAddressHandler(IUsersDbContext dbContext, ICurrentUser currentUser) : IRequestHandler<AddAddressRequest, Guid>
{
    public async Task<Guid> Handle(AddAddressRequest request, CancellationToken cancellationToken)
    {
        User? user = await dbContext.Users.GetById(currentUser.UserId, cancellationToken);

        Address address = request.ToAddress();

        user!.AddAddress(address);

        dbContext.Addresses.Add(address);

        await dbContext.SaveChangesAsync(cancellationToken);

        return address.Id;
    }
}
