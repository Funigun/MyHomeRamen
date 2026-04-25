using FluentValidation;
using MyHomeRamen.Api.Common.Authorization;
using MyHomeRamen.Common.Contracts.Account;
using MyHomeRamen.Domain.Common.Address;
using MyHomeRamen.Domain.Users.Database;
using MyHomeRamen.Identity.Api.Features.Account.Addresses.AddAddress.Models;
using MyHomeRamen.Persistance.Users.Extensions;

namespace MyHomeRamen.Identity.Api.Features.Account.Addresses.AddAddress.Policies;

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
