using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.Features.Identity.Abstractions;

namespace MyHomeRamen.Features.Identity.Features.Users.UpdateAddress;

public sealed record UpdateAddressCommand(Guid Id, UpdateAddressRequest Request) : ICommand<UpdateAddressResponse>;

public sealed class UpdateAddressHandler(IIdentityDbContext dbContext, ICurrentUser currentUser) : ICommandHandler<UpdateAddressCommand, UpdateAddressResponse>
{
    public async Task<UpdateAddressResponse> Handle(UpdateAddressCommand command, CancellationToken cancellationToken)
    {
        User? user = await dbContext.User.Specification().ByExternalId(currentUser.Id, cancellationToken);

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

