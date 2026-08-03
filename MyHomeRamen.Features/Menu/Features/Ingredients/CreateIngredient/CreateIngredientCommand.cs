using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Features.Menu.Features.Abstractions;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.CreateIngredient;

public sealed record CreateIngredientCommand(CreateIngredientRequest CreateIngredientRequest) : ICommand<CreateIngredientResponse>;

public sealed class CreateIngredientHandler(IMenuDbContext dbContext) : ICommandHandler<CreateIngredientCommand, CreateIngredientResponse>
{
    public async Task<CreateIngredientResponse> Handle(CreateIngredientCommand command, CancellationToken cancellationToken)
    {
        IEnumerable<Category> categories = await dbContext.Category.Query()
                                                                   .GetByIds(command.CreateIngredientRequest.CategoryIds.Select(id => (CategoryId)id), cancellationToken);

        Ingredient ingredient = command.CreateIngredientRequest.ToDomain(categories);

        dbContext.Ingredient.Add(ingredient);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateIngredientResponse(ingredient.Id.Value);
    }
}

