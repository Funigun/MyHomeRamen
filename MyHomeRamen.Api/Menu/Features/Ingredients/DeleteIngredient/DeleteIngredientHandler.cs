using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.DeleteIngredient;

public sealed class DeleteIngredientHandler(IMenuDbContext dbContext) : IRequestHandler<DeleteIngredientCommand, IResult>
{
    public async Task<IResult> Handle([FromRoute] DeleteIngredientCommand id, CancellationToken cancellationToken)
    {
        Ingredient ingredient = await dbContext.Ingredients.GetById((IngredientId)id.Id, cancellationToken);

        dbContext.Ingredients.Remove(ingredient);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }
}
