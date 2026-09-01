using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.GetIngredientsForDropdown;

public sealed record GetIngredientsForDropdownQuery : IQuery<GetIngredientsForDropdownResponse>;

public sealed record GetIngredientsForDropdownQueryOptions() : DbQueryOptions<Ingredient, IngredientForDropdownDto>
(
    new()
    {
        OrderBy = ingredient => ingredient.Name,
        OrderDirection = "asc",
        Selector = ingredient => new IngredientForDropdownDto(ingredient.Id.Value, ingredient.Name)
    }
);

public sealed class GetIngredientsForDropdownAuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<GetIngredientsForDropdownQuery>
{
    public async Task<bool> Authorize(GetIngredientsForDropdownQuery request, CancellationToken cancellationToken)
    {
        bool canManageIngredients = currentUser.CanManageIngredients() && currentUser.CanEditIngredient();
        bool canManageProducts = currentUser.CanManageProducts() && currentUser.CanEditProduct();

        return canManageIngredients || canManageProducts;
    }
}

public sealed class GetIngredientsForDropdownHandler(IMenuDbContext dbContext): IRequestHandler<GetIngredientsForDropdownQuery, GetIngredientsForDropdownResponse>
{
    public async Task<GetIngredientsForDropdownResponse> Handle(GetIngredientsForDropdownQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<IngredientForDropdownDto> ingredients = await dbContext.Ingredient.Query().ForDropdown(new GetIngredientsForDropdownQueryOptions(), cancellationToken);

        return new GetIngredientsForDropdownResponse(ingredients);
    }
}
