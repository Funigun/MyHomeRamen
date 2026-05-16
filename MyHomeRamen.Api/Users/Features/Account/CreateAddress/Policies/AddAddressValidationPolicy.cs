using FluentValidation;
using MyHomeRamen.Api.Common.Authorization;
using MyHomeRamen.Api.Users.Features.Account.CreateAddress.Models;
using MyHomeRamen.Common.Contracts.Account;
using MyHomeRamen.Domain.Common.Address;
using MyHomeRamen.Domain.Users.Database;
using MyHomeRamen.Persistance.Users.Extensions;

namespace MyHomeRamen.Api.Users.Features.Account.CreateAddress.Policies;

public sealed class AddAddressValidationPolicy : AbstractValidator<AddAddressRequest>
{
    public AddAddressValidationPolicy(IUsersDbContext dbContext, ICurrentUser currentUser)
    {
        RuleFor(x => x.Street)
            .ValidStreet();

        RuleFor(x => x.Building)
            .ValidBuilding();

        RuleFor(x => x.Apartment)
            .ValidApartment();

        RuleFor(x => x.City)
            .ValidCity();

        RuleFor(x => x.ZipCode)
            .ValidZipCode();

        RuleFor(x => x)
          .MustAsync(async (request, cancellationToken) =>
          {
              int addressCount = await dbContext.Users.GetNumberOfAddresses(currentUser.UserId, cancellationToken);
              return addressCount < AddressConstants.MaxAddressesPerUser;
          })
          .WithMessage($"User cannot have more than {AddressConstants.MaxAddressesPerUser} addresses.");
    }
}
