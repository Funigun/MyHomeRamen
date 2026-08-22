using FluentValidation;
using MyHomeRamen.Domain.Common.Address;
using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Identity.Abstractions;
using MyHomeRamen.Features.Identity.Features.Users.Common;

namespace MyHomeRamen.Features.Identity.Features.Users.CreateAddress;

public sealed record CreateAddressCommand(CreateAddressRequest Request) : ICommand<CreateAddressResponse>;

public sealed class CreateAddressAuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<CreateAddressCommand>
{
    public async Task<bool> Authorize(CreateAddressCommand request, CancellationToken cancellationToken)
    {
        return currentUser.CanEditUserProfile();
    }
}

public sealed class CreateAddressValidator : AbstractValidator<CreateAddressCommand>
{
    public CreateAddressValidator(IIdentityDbContext dbContext, ICurrentUser currentUser)
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
              int addressCount = await dbContext.User.Query().GetNumberOfAddresses(currentUser.UserId, cancellationToken);
              return addressCount < AddressConstants.MaxAddressesPerUser;
          })
          .WithMessage($"User cannot have more than {AddressConstants.MaxAddressesPerUser} addresses.");
    }
}

public sealed class CreateAddressHandler(IIdentityDbContext dbContext, ICurrentUser currentUser) : ICommandHandler<CreateAddressCommand, CreateAddressResponse>
{
    public async Task<CreateAddressResponse> Handle(CreateAddressCommand command, CancellationToken cancellationToken)
    {
        User? user = await dbContext.User.Load().ById(currentUser.UserId, cancellationToken);

        Address address = command.Request.ToAddress();

        user!.AddAddress(address);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateAddressResponse(address.Id);
    }
}

internal static class Mappings
{
    extension(CreateAddressRequest request)
    {
        internal Address ToAddress()
        {
            return Address.Create(
                Guid.CreateVersion7(),
                request.Street,
                request.Building,
                request.Apartment ?? string.Empty,
                request.City,
                request.ZipCode,
                request.IsDefault);
        }
    }
}
