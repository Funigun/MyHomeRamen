using FluentValidation;
using MyHomeRamen.Api.Common.Authorization;
using MyHomeRamen.Domain.Users.Database;
using MyHomeRamen.Persistance.Users.Extensions;

namespace MyHomeRamen.Api.Users.Features.Account.DeleteAddress;

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
