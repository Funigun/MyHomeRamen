using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientById;

public sealed class GetIngredientByIdHandler(IMenuDbContext dbContext)
    : IRequestHandler<GetIngredientByIdQuery, GetIngredientByIdResponse>
{
    public async Task<GetIngredientByIdResponse> Handle(GetIngredientByIdQuery request, CancellationToken cancellationToken)
    {
        IngredientId ingredientId = request.Id;

        Ingredient ingredient = await dbContext.Ingredients
            .Include(i => i.Categories)
            .AsSplitQuery()
            .GetByIdQuery(ingredientId, cancellationToken);

        return ingredient.ToResponse();
    }
}
