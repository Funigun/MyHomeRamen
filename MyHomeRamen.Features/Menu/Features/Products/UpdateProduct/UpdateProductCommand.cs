using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Features.Menu.Features.Abstractions;

namespace MyHomeRamen.Features.Menu.Features.Products.UpdateProduct;

public sealed record UpdateProductCommand(ProductId Id, UpdateProductRequest UpdateProductRequest) : ICommand<UpdateProductResponse>;

public sealed class UpdateProductHandler(IMenuDbContext dbContext) : ICommandHandler<UpdateProductCommand, UpdateProductResponse>
{
    public async Task<UpdateProductResponse> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        Product product = await dbContext.Product.Specification().ById(command.Id, cancellationToken);

        Category category = await dbContext.Category.Specification().ById(command.UpdateProductRequest.CategoryId, cancellationToken);

        IEnumerable<IngredientId> ingredientIds = command.UpdateProductRequest.IngredientIds.Select(id => (IngredientId)id);
        IEnumerable<Ingredient> ingredients = await dbContext.Ingredient.Specification().ByIds(ingredientIds, cancellationToken);

        IEnumerable<IngredientId> customIngredientIds = command.UpdateProductRequest.CustomIngredientIds.Select(id => (IngredientId)id);
        IEnumerable<Ingredient> customIngredients = await dbContext.Ingredient.Specification().ByIds(customIngredientIds, cancellationToken);

        product.Update(command.UpdateProductRequest.Name, command.UpdateProductRequest.Description ?? string.Empty, command.UpdateProductRequest.Price, category, ingredients, customIngredients);

        await dbContext.SaveChangesAsync(cancellationToken);

        return product.ToResponse();
    }
}

