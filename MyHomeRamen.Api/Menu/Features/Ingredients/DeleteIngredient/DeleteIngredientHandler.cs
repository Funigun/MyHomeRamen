using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Ingredients.DeleteIngredient.Models;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.DeleteIngredient;

public sealed class DeleteIngredientHandler(IMenuDbContext dbContext) : IRequestHandler<DeleteIngredientRequest, IResult>
{
    public async Task<IResult> Handle([FromRoute] DeleteIngredientRequest id, CancellationToken cancellationToken)
    {
        Ingredient ingredient = await dbContext.Ingredients.GetBySelectorAsync((IngredientId)id.Id, cancellationToken);

        dbContext.Ingredients.Remove(ingredient);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }
}
