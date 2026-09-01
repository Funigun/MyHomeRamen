using FluentValidation;
using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Identity.Abstractions;
using MyHomeRamen.Features.Identity.Features.Users.Common;

namespace MyHomeRamen.Features.Identity.Features.Users.UpdateAddress;

public sealed record UpdateAddressCommand(Guid Id, UpdateAddressRequest Request) : ICommand<UpdateAddressResponse>;

public sealed class UpdateAddressAuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<UpdateAddressCommand>
{
    public async Task<bool> Authorize(UpdateAddressCommand request, CancellationToken cancellationToken)
    {
        return currentUser.CanEditUserProfile();
    }
}

public sealed class UpdateAddressValidator : AbstractValidator<UpdateAddressCommand>
{
    public UpdateAddressValidator(IIdentityDbContext dbContext, ICurrentUser currentUser)
    {
        RuleFor(x => x.Request.Street)
            .ValidStreet();

        RuleFor(x => x.Request.Building)
            .ValidBuilding();

        RuleFor(x => x.Request.Apartment)
            .ValidApartment();

        RuleFor(x => x.Request.City)
            .ValidCity();

        RuleFor(x => x.Request.ZipCode)
            .ValidZipCode();

        RuleFor(x => x)
          .MustAsync(async (command, cancellationToken) =>
          {
              return await dbContext.User.Query().AddressExists(currentUser.UserId, command.Id, cancellationToken);
          })
          .WithMessage($"Address not found or does not belong to the current user.");
    }
}
public sealed class UpdateAddressHandler(IIdentityDbContext dbContext, ICurrentUser currentUser) : ICommandHandler<UpdateAddressCommand, UpdateAddressResponse>
{
    public async Task<UpdateAddressResponse> Handle(UpdateAddressCommand command, CancellationToken cancellationToken)
    {
        User? user = await dbContext.User.Load().ById(new UserId(currentUser.UserId), cancellationToken);

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
