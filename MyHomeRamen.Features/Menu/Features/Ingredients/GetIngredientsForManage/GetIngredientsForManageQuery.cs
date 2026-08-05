using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Features.Common.Endpoints.Models;
using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Menu.Features.Abstractions;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.GetIngredientsForManage;

public sealed record GetIngredientsForManageQuery(GetIngredientsForManageRequest Request, PageParameters PageParameters) : IQuery<GetIngredientsForManageResponse>;

public sealed record GetIngredientsForManageQueryOptions(string? Name, IEnumerable<Guid>? CategoryIds, PageParameters PageParameters) 
                    : PagedDbQueryOptions<Ingredient, IngredientForManageDto>
                    (
                        new()
                        {
                            Filter = ingredient =>
                                (string.IsNullOrWhiteSpace(Name) || ingredient.Name.ToLower().Contains(Name.ToLower())) &&
                                (CategoryIds == null || ingredient.Categories.Any(category => CategoryIds.Contains(category.Id.Value))),
                            OrderBy = ingredient => ingredient.Name,
                            OrderDirection = "asc",
                            PageNumber = PageParameters.PageNumber,
                            PageSize = PageParameters.PageSize,
                            Selector = ingredient => new IngredientForManageDto(ingredient.Id.Value, ingredient.Name, ingredient.Description)
                        }
                    );

public sealed class GetIngredientsForManageHandler(IMenuDbContext dbContext) : IQueryHandler<GetIngredientsForManageQuery, GetIngredientsForManageResponse>
{
    public async Task<GetIngredientsForManageResponse> Handle(GetIngredientsForManageQuery query, CancellationToken cancellationToken)
    {
        GetIngredientsForManageQueryOptions options = new(query.Request.Name, query.Request.CategoryIds, query.PageParameters);

        PagedResult<IngredientForManageDto> result = await dbContext.Ingredient.Query().ForManage(options, cancellationToken);

        return new GetIngredientsForManageResponse
        (
            Page: query.PageParameters.PageNumber,
            PageSize: query.PageParameters.PageSize,
            TotalCount: result.TotalItems,
            Ingredients: result.Items
        );
    }
}

