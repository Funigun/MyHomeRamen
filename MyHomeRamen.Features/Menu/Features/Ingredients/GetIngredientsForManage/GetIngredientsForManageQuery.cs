using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Features.Common.Endpoints.Models;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Features.Menu.Features.Abstractions;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.GetIngredientsForManage;

public sealed record GetIngredientsForManageQuery(GetIngredientsForManageRequest Request, PageParameters PageParameters)
                   : IQuery<GetIngredientsForManageResponse>;

public sealed class GetIngredientsForManageHandler(IMenuDbContext dbContext) : IQueryHandler<GetIngredientsForManageQuery, GetIngredientsForManageResponse>
{
    public async Task<GetIngredientsForManageResponse> Handle(GetIngredientsForManageQuery query, CancellationToken cancellationToken)
    {
        List<Ingredient> ingredients = await dbContext.Ingredient.Query()
                                                                 .GetForManage(query.Request.Name, query.Request.CategoryIds, cancellationToken);

        int totalCount = ingredients.Count;

        ingredients = ingredients.OrderBy(ingredient => ingredient.Name)
                                 .Skip((query.PageParameters.PageNumber - 1) * query.PageParameters.PageSize)
                                 .Take(query.PageParameters.PageSize)
                                 .ToList();

        List<IngredientForManageDto> ingredientDtos = ingredients.Select(ingredient => ingredient.ToResponse()).ToList();

        return new GetIngredientsForManageResponse(
            Page: query.PageParameters.PageNumber,
            PageSize: query.PageParameters.PageSize,
            TotalCount: totalCount,
            Ingredients: ingredientDtos);
    }
}

