using FluentValidation;
using MyHomeRamen.Api.Common.Authorization;
using MyHomeRamen.Common.Contracts.Account;
using MyHomeRamen.Domain.Common.Address;
using MyHomeRamen.Domain.Users.Database;
using MyHomeRamen.Persistance.Users.Extensions;

namespace MyHomeRamen.Api.Users.Features.Account.CreateAddress;

public sealed class CreateAddressValidator : AbstractValidator<CreateAddressCommand>
{
    public CreateAddressValidator(IUsersDbContext dbContext, ICurrentUser currentUser)
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
              int addressCount = await dbContext.Users.GetNumberOfAddresses(currentUser.UserId, cancellationToken);
              return addressCount < AddressConstants.MaxAddressesPerUser;
          })
          .WithMessage($"User cannot have more than {AddressConstants.MaxAddressesPerUser} addresses.");
    }
}
