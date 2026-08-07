using FluentValidation;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Identity.Features.Users.Common;
using MyHomeRamen.Features.Identity.Abstractions;

namespace MyHomeRamen.Features.Identity.Features.Users.UpdateAddress;

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
