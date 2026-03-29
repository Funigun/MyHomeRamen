using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Ingredients.CreateIngredient.Models;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.CreateIngredient;

public sealed class CreateIngredientHandler(IMenuDbContext dbContext) : IRequestHandler<CreateIngredientRequest, Guid>
{
    public async Task<Guid> Handle(CreateIngredientRequest request, CancellationToken cancellationToken)
    {
        IEnumerable<Category> categories = await dbContext.Categories
            .GetByIds(request.CategoryIds.Select(id => (CategoryId)id), cancellationToken);

        Ingredient ingredient = request.ToDomain(categories);

        dbContext.Ingredients.Add(ingredient);
        await dbContext.SaveChangesAsync(cancellationToken);

        return (Guid)ingredient.Id;
    }
}
