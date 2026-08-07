using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Menu.Features.Abstractions;

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

public sealed class GetIngredientsForDropdownHandler(IMenuDbContext dbContext): IQueryHandler<GetIngredientsForDropdownQuery, GetIngredientsForDropdownResponse>
{
    public async Task<GetIngredientsForDropdownResponse> Handle(GetIngredientsForDropdownQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<IngredientForDropdownDto> ingredients = await dbContext.Ingredient.Query().ForDropdown(new GetIngredientsForDropdownQueryOptions(), cancellationToken);

        return new GetIngredientsForDropdownResponse(ingredients);
    }
}

