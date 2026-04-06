using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Exceptions;
using MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientById.Models;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Domain.Menu.Ingredients;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientById;

public sealed class GetIngredientByIdHandler(IMenuDbContext dbContext)
    : IRequestHandler<GetIngredientByIdRequest, GetIngredientByIdResponse>
{
    public async Task<GetIngredientByIdResponse> Handle(GetIngredientByIdRequest request, CancellationToken cancellationToken)
    {
        IngredientId ingredientId = request.Id;

        Ingredient? ingredient = await dbContext.Ingredients
            .AsNoTracking()
            .Include(i => i.Categories)
            .FirstOrDefaultAsync(i => i.Id == ingredientId, cancellationToken);

        if (ingredient is null)
        {
            throw new IngredientNotFoundException(request.Id);
        }

        return ingredient.ToResponse();
    }
}
