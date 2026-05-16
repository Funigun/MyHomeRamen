using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.DeleteIngredient;

public sealed class DeleteIngredientHandler(IMenuDbContext dbContext) : ICommandHandler<DeleteIngredientCommand, IResult>
{
    public async Task<IResult> Handle(DeleteIngredientCommand id, CancellationToken cancellationToken)
    {
        Ingredient ingredient = await dbContext.Ingredients.GetById((IngredientId)id.Id, cancellationToken);

        dbContext.Ingredients.Remove(ingredient);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }
}
