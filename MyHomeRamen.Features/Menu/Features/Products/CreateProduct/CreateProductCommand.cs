using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Features.Menu.Features.Abstractions;

namespace MyHomeRamen.Features.Menu.Features.Products.CreateProduct;

public sealed record CreateProductCommand(CreateProductRequest CreateProductRequest) : ICommand<CreateProductResponse>;

public sealed class CreateProductHandler(IMenuDbContext dbContext) : ICommandHandler<CreateProductCommand, CreateProductResponse>
{
    public async Task<CreateProductResponse> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        Category category = await dbContext.Category.Specification().ById((CategoryId)command.CreateProductRequest.CategoryId, cancellationToken);

        IEnumerable<Ingredient> ingredients = await dbContext.Ingredient.Specification().ByIds(command.CreateProductRequest.IngredientIds.Select(id => (IngredientId)id), cancellationToken);

        IEnumerable<Ingredient> customIngredients = await dbContext.Ingredient.Specification().ByIds(command.CreateProductRequest.CustomIngredientIds.Select(id => (IngredientId)id), cancellationToken);

        Product product = command.CreateProductRequest.ToDomain(category, ingredients, customIngredients);

        dbContext.Product.Add(product);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateProductResponse(product.Id.Value);
    }
}

