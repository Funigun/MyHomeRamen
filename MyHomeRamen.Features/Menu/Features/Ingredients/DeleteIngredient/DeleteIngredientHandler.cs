using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.DeleteIngredient;

public sealed class DeleteIngredientHandler(IMenuDbContext dbContext) : ICommandHandler<DeleteIngredientCommand>
{
    public async Task Handle(DeleteIngredientCommand id, CancellationToken cancellationToken)
    {
        Ingredient ingredient = await dbContext.Ingredients.GetById((IngredientId)id.Id, cancellationToken);

        dbContext.Ingredients.Remove(ingredient);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
