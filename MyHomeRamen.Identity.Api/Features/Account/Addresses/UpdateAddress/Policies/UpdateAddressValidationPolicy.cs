using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Authorization;
using MyHomeRamen.Common.Contracts.Account;
using MyHomeRamen.Domain.Users.Database;
using MyHomeRamen.Identity.Api.Features.Account.Addresses.UpdateAddress.Models;

namespace MyHomeRamen.Identity.Api.Features.Account.Addresses.UpdateAddress.Policies;

public sealed class UpdateAddressValidationPolicy : AbstractValidator<UpdateAddressRequest>
{
    public UpdateAddressValidationPolicy(IUsersDbContext dbContext, ICurrentUser currentUser)
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

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Address ID must not be empty.")
            .MustAsync(async (id, cancellationToken) =>
                await dbContext.Users
                    .AsNoTracking()
                    .Include(u => u.Addresses)
                    .AnyAsync(u => u.KeycloakUserId == currentUser.Id && u.Addresses.Any(a => a.Id == id), cancellationToken))
            .WithMessage("Address not found or does not belong to the current user.");
    }
}
