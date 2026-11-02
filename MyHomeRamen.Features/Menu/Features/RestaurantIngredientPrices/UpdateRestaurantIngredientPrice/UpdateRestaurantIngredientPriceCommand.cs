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

namespace MyHomeRamen.Features.Menu.Features.RestaurantIngredientPrices.UpdateRestaurantIngredientPrice;

public sealed record UpdateRestaurantIngredientPriceCommand(Guid IngredientId, decimal Price)
    : ICommand<UpdateRestaurantIngredientPriceResponse>;

public sealed class UpdateRestaurantIngredientPriceAuthorizationPolicy(
    ICurrentUser currentUser,
    ICompanyContext companyContext)
    : IAuthorizationPolicy<UpdateRestaurantIngredientPriceCommand>
{
    public Task<bool> Authorize(
        UpdateRestaurantIngredientPriceCommand request,
        CancellationToken cancellationToken)
        => Task.FromResult(MenuAuthorization.CanManageRestaurant(
            currentUser,
            companyContext.RestaurantId,
            MenuPermissionConstants.CanManageIngredientPrices,
            MenuPermissionConstants.CanEditIngredientPrice));
}

public sealed class UpdateRestaurantIngredientPriceValidator : AbstractValidator<UpdateRestaurantIngredientPriceCommand>
{
    public UpdateRestaurantIngredientPriceValidator(IMenuDbContext dbContext)
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

        RuleFor(command => command.Price)
            .GreaterThanOrEqualTo(0m)
            .WithMessage("Extra ingredient price must be greater than or equal to zero.");
    }
}

public sealed class UpdateRestaurantIngredientPriceHandler(IMenuDbContext dbContext)
    : IRequestHandler<UpdateRestaurantIngredientPriceCommand, UpdateRestaurantIngredientPriceResponse>
{
    public async Task<UpdateRestaurantIngredientPriceResponse> Handle(
        UpdateRestaurantIngredientPriceCommand command,
        CancellationToken cancellationToken)
    {
        RestaurantIngredientPrice? price = await dbContext.RestaurantIngredientPrice.Load()
            .ByIngredientId(new IngredientId(command.IngredientId), cancellationToken);
        if (price is null)
        {
            throw new InvalidOperationException("The restaurant ingredient price could not be loaded after validation.");
        }

        price.UpdatePrice(command.Price);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateRestaurantIngredientPriceResponse(command.IngredientId);
    }
}
