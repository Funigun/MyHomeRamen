using FluentValidation;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.RestaurantIngredientPrices;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Common.Tenancy;
using MyHomeRamen.Features.Identity.Permissions;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Authorization;

namespace MyHomeRamen.Features.Menu.Features.RestaurantIngredientPrices.RemoveRestaurantIngredientPrice;

public sealed record RemoveRestaurantIngredientPriceCommand(Guid IngredientId) : ICommand;

public sealed class RemoveRestaurantIngredientPriceAuthorizationPolicy(
    ICurrentUser currentUser,
    ICompanyContext companyContext)
    : IAuthorizationPolicy<RemoveRestaurantIngredientPriceCommand>
{
    public Task<bool> Authorize(
        RemoveRestaurantIngredientPriceCommand request,
        CancellationToken cancellationToken)
        => Task.FromResult(MenuAuthorization.CanManageRestaurant(
            currentUser,
            companyContext.RestaurantId,
            MenuPermissionConstants.CanManageIngredientPrices,
            MenuPermissionConstants.CanDeleteIngredientPrice));
}

public sealed class RemoveRestaurantIngredientPriceValidator : AbstractValidator<RemoveRestaurantIngredientPriceCommand>
{
    public RemoveRestaurantIngredientPriceValidator(IMenuDbContext dbContext)
    {
        RuleFor(command => command.IngredientId)
            .NotEmpty()
            .WithMessage("Ingredient ID must not be empty.")
            .MustAsync(async (ingredientId, cancellationToken) =>
                await dbContext.RestaurantIngredientPrice.Exists(
                    price => price.RestaurantId == dbContext.RestaurantId
                        && price.IngredientId == new IngredientId(ingredientId),
                    cancellationToken))
            .WithMessage("An extra ingredient price at this restaurant must exist.");
    }
}

public sealed class RemoveRestaurantIngredientPriceHandler(IMenuDbContext dbContext)
    : IRequestHandler<RemoveRestaurantIngredientPriceCommand, Unit>
{
    public async Task<Unit> Handle(
        RemoveRestaurantIngredientPriceCommand command,
        CancellationToken cancellationToken)
    {
        RestaurantIngredientPrice? price = await dbContext.RestaurantIngredientPrice.Load()
            .ByIngredientId(new IngredientId(command.IngredientId), cancellationToken);
        if (price is null)
        {
            throw new InvalidOperationException("The restaurant ingredient price could not be loaded after validation.");
        }

        dbContext.RestaurantIngredientPrice.Delete(price);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
