using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Ingredients.CreateIngredient.Models;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Domain.Menu.Ingredients;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.CreateIngredient;

public sealed class CreateIngredientHandler(IMenuDbContext dbContext) : IRequestHandler<CreateIngredientRequest, Guid>
{
    public async Task<Guid> Handle(CreateIngredientRequest request, CancellationToken cancellationToken)
    {
        List<Category> categories = await dbContext.Categories
            .Where(c => request.CategoryIds.Contains(c.Id.Value))
            .ToListAsync(cancellationToken);

        Ingredient ingredient = request.ToDomain(categories);

        dbContext.Ingredients.Add(ingredient);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ingredient.Id.Value;
    }
}
