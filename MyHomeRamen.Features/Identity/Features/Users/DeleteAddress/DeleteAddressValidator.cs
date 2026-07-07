using FluentValidation;
using MyHomeRamen.Features.Identity.Extensions;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Identity.Abstractions;

namespace MyHomeRamen.Features.Identity.Features.Users.DeleteAddress;

public sealed class DeleteAddressValidator : AbstractValidator<DeleteAddressCommand>
{
    public DeleteAddressValidator(IIdentityDbContext dbContext, ICurrentUser currentUser)
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Address ID must not be empty.")
            .MustAsync(async (id, cancellationToken) =>
                await dbContext.User.Query().AddressExists(currentUser.UserId, id, cancellationToken))
            .WithMessage("Address not found or does not belong to the current user.");
    }
}

