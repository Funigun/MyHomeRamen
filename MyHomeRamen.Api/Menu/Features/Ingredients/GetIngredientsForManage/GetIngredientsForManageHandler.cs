using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientsForManage.Models;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientsForManage;

public sealed class GetIngredientsForManageHandler(IMenuDbContext dbContext)
    : IRequestHandler<GetIngredientsForManageRequest, GetIngredientsForManageResponse>
{
    public async Task<GetIngredientsForManageResponse> Handle(
        GetIngredientsForManageRequest request,
        CancellationToken cancellationToken)
    {
        IQueryable<Ingredient> query = dbContext.Ingredients.ForManage(request.Name, request.CategoryIds);

        int totalCount = await query.CountAsync(cancellationToken);

        query = query.OrderBy(ingredient => ingredient.Name)
                     .Paged(request.PageParameters.PageNumber, request.PageParameters.PageSize);

        List<IngredientDto> ingredients = await query.Select(ingredient => ingredient.ToResponse()).ToListAsync(cancellationToken);

        return new GetIngredientsForManageResponse(
            Page: request.PageParameters.PageNumber,
            PageSize: request.PageParameters.PageSize,
            TotalCount: totalCount,
            Ingredients: ingredients);
    }
}
