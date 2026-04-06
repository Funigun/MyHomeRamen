using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientById.Models;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientById;

public sealed class GetIngredientByIdHandler(IMenuDbContext dbContext)
    : IRequestHandler<GetIngredientByIdRequest, GetIngredientByIdResponse>
{
    public async Task<GetIngredientByIdResponse> Handle(GetIngredientByIdRequest request, CancellationToken cancellationToken)
    {
        Ingredient ingredient = await dbContext.Ingredients.GetBySelectorNotTrackedAsync((IngredientId)request.Id, cancellationToken);
        return ingredient.ToResponse();
    }
}
