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

namespace MyHomeRamen.Features.Menu.Features.RestaurantIngredientPrices.CreateRestaurantIngredientPrice;

public sealed record CreateRestaurantIngredientPriceCommand(Guid IngredientId, decimal Price)
    : ICommand<CreateRestaurantIngredientPriceResponse>;

public sealed class CreateRestaurantIngredientPriceAuthorizationPolicy(
    ICurrentUser currentUser,
    ICompanyContext companyContext)
    : IAuthorizationPolicy<CreateRestaurantIngredientPriceCommand>
{
    public Task<bool> Authorize(
        CreateRestaurantIngredientPriceCommand request,
        CancellationToken cancellationToken)
        => Task.FromResult(MenuAuthorization.CanManageRestaurant(
            currentUser,
            companyContext.RestaurantId,
            MenuPermissionConstants.CanManageIngredientPrices,
            MenuPermissionConstants.CanAddIngredientPrice));
}

public sealed class CreateRestaurantIngredientPriceValidator : AbstractValidator<CreateRestaurantIngredientPriceCommand>
{
    public CreateRestaurantIngredientPriceValidator(IMenuDbContext dbContext)
    {
        RuleFor(command => command.IngredientId)
            .NotEmpty()
            .WithMessage("Ingredient ID must not be empty.")
            .MustAsync(async (ingredientId, cancellationToken) =>
                await dbContext.Ingredient.Exists(
                    ingredient => ingredient.Id == new IngredientId(ingredientId),
                    cancellationToken))
            .WithMessage("Ingredient must exist in this company.")
            .MustAsync(async (ingredientId, cancellationToken) =>
                !await dbContext.RestaurantIngredientPrice.Exists(
                    price => price.RestaurantId == dbContext.RestaurantId
                        && price.IngredientId == new IngredientId(ingredientId),
                    cancellationToken))
            .WithMessage("This ingredient already has an extra price at this restaurant.");

        RuleFor(command => command.Price)
            .GreaterThanOrEqualTo(0m)
            .WithMessage("Extra ingredient price must be greater than or equal to zero.");
    }
}

public sealed class CreateRestaurantIngredientPriceHandler(IMenuDbContext dbContext)
    : IRequestHandler<CreateRestaurantIngredientPriceCommand, CreateRestaurantIngredientPriceResponse>
{
    public async Task<CreateRestaurantIngredientPriceResponse> Handle(
        CreateRestaurantIngredientPriceCommand command,
        CancellationToken cancellationToken)
    {
        Ingredient ingredient = await dbContext.Ingredient.Load()
            .ById(new IngredientId(command.IngredientId), cancellationToken);
        RestaurantIngredientPrice price = RestaurantIngredientPrice.Create(
            new RestaurantIngredientPriceId(Guid.CreateVersion7()),
            dbContext.RestaurantId,
            ingredient,
            command.Price);

        dbContext.RestaurantIngredientPrice.Add(price);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateRestaurantIngredientPriceResponse(price.Id.Value);
    }
}
