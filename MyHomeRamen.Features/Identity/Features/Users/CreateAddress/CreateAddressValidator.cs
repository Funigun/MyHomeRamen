using FluentValidation;
using MyHomeRamen.Domain.Common.Address;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Common.Contracts.Users.Account.Validators;
using MyHomeRamen.Features.Identity.Abstractions;

namespace MyHomeRamen.Features.Identity.Features.Users.CreateAddress;

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

