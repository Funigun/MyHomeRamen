using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Features.Menu.Features.Abstractions;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.GetIngredientsForDropdown;

public sealed class GetIngredientsForDropdownHandler(IMenuDbContext dbContext)
    : IQueryHandler<GetIngredientsForDropdownQuery, IEnumerable<GetIngredientsForDropdownResponse>>
{
    public async Task<IEnumerable<GetIngredientsForDropdownResponse>> Handle(GetIngredientsForDropdownQuery request, CancellationToken cancellationToken)
    {
        List<Ingredient> ingredients = await dbContext.Ingredient.Query()
                                                                 .GetForDropdown(cancellationToken);

        return ingredients.Select(i => i.ToResponse());
    }
}
