using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.CreateIngredient;

public sealed class CreateIngredientHandler(IMenuDbContext dbContext) : IRequestHandler<CreateIngredientCommand, CreateIngredientResponse>
{
    public async Task<CreateIngredientResponse> Handle(CreateIngredientCommand command, CancellationToken cancellationToken)
    {
        IEnumerable<Category> categories = await dbContext.Categories
            .GetByIds(command.CreateIngredientRequest.CategoryIds.Select(id => (CategoryId)id), cancellationToken);

        Ingredient ingredient = command.CreateIngredientRequest.ToDomain(categories);

        dbContext.Ingredients.Add(ingredient);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateIngredientResponse(ingredient.Id.Value);
    }
}
