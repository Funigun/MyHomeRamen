using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.DTOs;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientsForManage;

public sealed class GetIngredientsForManageHandler(IMenuDbContext dbContext)
    : IQueryHandler<GetIngredientsForManageQuery, GetIngredientsForManageResponse>
{
    public async Task<GetIngredientsForManageResponse> Handle(
        GetIngredientsForManageQuery query,
        CancellationToken cancellationToken)
    {
        IQueryable<Ingredient> ingredientQuery = dbContext.Ingredients.ForManage(query.Request.Name, query.Request.CategoryIds);

        int totalCount = await ingredientQuery.CountAsync(cancellationToken);

        ingredientQuery = ingredientQuery.OrderBy(ingredient => ingredient.Name)
                     .Paged(query.PageParameters.PageNumber, query.PageParameters.PageSize);

        List<IngredientForManageDto> ingredients = await ingredientQuery.Select(ingredient => ingredient.ToResponse()).ToListAsync(cancellationToken);

        return new GetIngredientsForManageResponse(
            Page: query.PageParameters.PageNumber,
            PageSize: query.PageParameters.PageSize,
            TotalCount: totalCount,
            Ingredients: ingredients);
    }
}
