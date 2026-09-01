using FluentValidation;
using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Identity.Abstractions;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Identity.Features.Users.DeleteAddress;

public sealed record DeleteAddressCommand(Guid Id) : ICommand;

public sealed class DeleteAddressAuthorizationPolicy(ICurrentUser currentUser, IIdentityDbContext dbContext) : IAuthorizationPolicy<DeleteAddressCommand>
{
    public async Task<bool> Authorize(DeleteAddressCommand request, CancellationToken cancellationToken)
    {
        return currentUser.CanEditUserProfile() 
            && await dbContext.User.Query().AddressExists(currentUser.UserId, request.Id, cancellationToken);
    }
}

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

public sealed class DeleteAddressHandler(IIdentityDbContext dbContext, ICurrentUser currentUser) : IRequestHandler<DeleteAddressCommand, Unit>
{
    public async Task<Unit> Handle(DeleteAddressCommand command, CancellationToken cancellationToken)
    {
        User user = await dbContext.User.Load().ById(new UserId(currentUser.UserId), cancellationToken);

        user.RemoveAddress(command.Id);

        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
