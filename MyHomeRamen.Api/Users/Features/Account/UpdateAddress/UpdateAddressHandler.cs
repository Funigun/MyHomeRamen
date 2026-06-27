using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Common.Contracts.Users.Account.Responses;
using MyHomeRamen.Domain.Users;
using MyHomeRamen.Domain.Users.Database;
using MyHomeRamen.Features.Common.Authorization;

namespace MyHomeRamen.Api.Users.Features.Account.UpdateAddress;

public sealed class UpdateAddressHandler(IUsersDbContext dbContext, ICurrentUser currentUser) : ICommandHandler<UpdateAddressCommand, UpdateAddressResponse>
{
    public async Task<UpdateAddressResponse> Handle(UpdateAddressCommand command, CancellationToken cancellationToken)
    {
        User? user = await dbContext.Users
            .Include(u => u.Addresses)
            .FirstOrDefaultAsync(u => u.KeycloakUserId == currentUser.Id, cancellationToken);

        user!.UpdateAddress(
            command.Id,
            command.Request.Street,
            command.Request.Building,
            command.Request.Apartment ?? string.Empty,
            command.Request.City,
            command.Request.ZipCode,
            command.Request.IsDefault);

        await dbContext.SaveChangesAsync(cancellationToken);

        Address address = user.Addresses.First(a => a.Id == command.Id);

        return new UpdateAddressResponse(address.Id);
    }
}
