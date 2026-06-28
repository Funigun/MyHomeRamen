using FluentValidation;
using MyHomeRamen.Domain.Users.Database;
using MyHomeRamen.Features.Users.Extensions;
using MyHomeRamen.Features.Common.Authorization;

namespace MyHomeRamen.Features.Users.Features.Account.DeleteAddress;

public sealed class DeleteAddressValidator : AbstractValidator<DeleteAddressCommand>
{
    public DeleteAddressValidator(IUsersDbContext dbContext, ICurrentUser currentUser)
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Address ID must not be empty.")
            .MustAsync(async (id, cancellationToken) =>
                await dbContext.Users.AddressExists(currentUser.UserId, id, cancellationToken))
            .WithMessage("Address not found or does not belong to the current user.");
    }
}

