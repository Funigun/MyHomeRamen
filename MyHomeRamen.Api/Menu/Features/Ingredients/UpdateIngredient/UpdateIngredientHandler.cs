using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Ingredients.UpdateIngredient.Models;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.UpdateIngredient;

public sealed class UpdateIngredientHandler(IMenuDbContext dbContext) : IRequestHandler<UpdateIngredientRequest, UpdateIngredientResponse>
{
    public async Task<UpdateIngredientResponse> Handle(UpdateIngredientRequest request, CancellationToken cancellationToken)
    {
        Ingredient ingredient = await dbContext.Ingredients
            .Include(i => i.Categories)
            .AsSplitQuery()
            .GetById((IngredientId)request.Id, cancellationToken);

        IEnumerable<Category> categories = await dbContext.Categories.GetByIds(request.CategoryIds.Select(id => (CategoryId)id), cancellationToken);

        ingredient.Update(request.Name, request.Description, request.Price, categories);

        await dbContext.SaveChangesAsync(cancellationToken);

        return ingredient.ToResponse();
    }
}
