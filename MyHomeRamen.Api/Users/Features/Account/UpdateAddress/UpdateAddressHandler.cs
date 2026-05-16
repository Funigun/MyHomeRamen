using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Authorization;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Common.Contracts.Users.Account.Responses;
using MyHomeRamen.Domain.Users;
using MyHomeRamen.Domain.Users.Database;

namespace MyHomeRamen.Api.Users.Features.Account.UpdateAddress;

public sealed class UpdateAddressHandler(IUsersDbContext dbContext, ICurrentUser currentUser) : IRequestHandler<UpdateAddressCommand, UpdateAddressResponse>
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
