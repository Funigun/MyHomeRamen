using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Features.Menu.Features.Abstractions;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.DeleteIngredient;

public sealed class DeleteIngredientHandler(IMenuDbContext dbContext) : ICommandHandler<DeleteIngredientCommand>
{
    public async Task Handle(DeleteIngredientCommand id, CancellationToken cancellationToken)
    {
        Ingredient ingredient = await dbContext.Ingredient.Specification().ById((IngredientId)id.Id, cancellationToken);

        dbContext.Ingredient.Delete(ingredient);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
