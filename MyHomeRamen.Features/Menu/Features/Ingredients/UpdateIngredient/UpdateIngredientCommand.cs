using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Features.Menu.Features.Abstractions;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.UpdateIngredient;

public sealed record UpdateIngredientCommand(IngredientId Id, UpdateIngredientRequest UpdateIngredientRequest)
                   : ICommand<UpdateIngredientResponse>;

public sealed class UpdateIngredientHandler(IMenuDbContext dbContext) : ICommandHandler<UpdateIngredientCommand, UpdateIngredientResponse>
{
    public async Task<UpdateIngredientResponse> Handle(UpdateIngredientCommand request, CancellationToken cancellationToken)
    {
        Ingredient ingredient = await dbContext.Ingredient.Specification().ById(request.Id, cancellationToken);

        IEnumerable<Category> categories = await dbContext.Category.Specification().ByIds(request.UpdateIngredientRequest.CategoryIds.Select(id => (CategoryId)id), cancellationToken);

        ingredient.Update(request.UpdateIngredientRequest.Name, request.UpdateIngredientRequest.Description, request.UpdateIngredientRequest.Price, categories);

        await dbContext.SaveChangesAsync(cancellationToken);

        return ingredient.ToResponse();
    }
}

