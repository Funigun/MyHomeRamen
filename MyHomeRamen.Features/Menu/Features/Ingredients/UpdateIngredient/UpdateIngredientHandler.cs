using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.UpdateIngredient;

public sealed class UpdateIngredientHandler(IMenuDbContext dbContext) : ICommandHandler<UpdateIngredientCommand, UpdateIngredientResponse>
{
    public async Task<UpdateIngredientResponse> Handle(UpdateIngredientCommand request, CancellationToken cancellationToken)
    {
        Ingredient ingredient = await dbContext.Ingredients
            .Include(i => i.Categories)
            .AsSplitQuery()
            .GetById(request.Id, cancellationToken);

        IEnumerable<Category> categories = await dbContext.Categories.GetByIds(request.UpdateIngredientRequest.CategoryIds.Select(id => (CategoryId)id), cancellationToken);

        ingredient.Update(request.UpdateIngredientRequest.Name, request.UpdateIngredientRequest.Description, request.UpdateIngredientRequest.Price, categories);

        await dbContext.SaveChangesAsync(cancellationToken);

        return ingredient.ToResponse();
    }
}
