using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Authorization;
using MyHomeRamen.Domain.Users.Database;
using MyHomeRamen.Identity.Api.Features.Account.Addresses.DeleteAddress.Models;

namespace MyHomeRamen.Identity.Api.Features.Account.Addresses.DeleteAddress.Policies;

public sealed class DeleteAddressValidationPolicy : AbstractValidator<DeleteAddressRequest>
{
    public DeleteAddressValidationPolicy(IUsersDbContext dbContext, ICurrentUser currentUser)
    {
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
