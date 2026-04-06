using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Ingredients.UpdateIngredient.Models;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.UpdateIngredient;

public sealed class UpdateIngredientHandler(IMenuDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<UpdateIngredientRequest, UpdateIngredientResponse>
{
    public async Task<UpdateIngredientResponse> Handle(UpdateIngredientRequest request, CancellationToken cancellationToken)
    {
        Guid id = (Guid)httpContextAccessor.HttpContext!.GetRouteValue("id")!;

        Ingredient ingredient = await dbContext.Ingredients
            .Include(i => i.Categories)
            .GetBySelectorAsync((IngredientId)id, cancellationToken);

        IEnumerable<Category> categories = await dbContext.Categories
            .GetByIds(request.CategoryIds.Select(categoryId => (CategoryId)categoryId), cancellationToken);

        ingredient.Update(request.Name, request.Description, request.Price, new Collection<Category>(categories.ToList()));

        await dbContext.SaveChangesAsync(cancellationToken);

        return ingredient.ToResponse();
    }
}
