using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Common.Contracts.Menu.Products.Responses;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Products.CreateProduct;

public sealed class CreateProductHandler(IMenuDbContext dbContext) : ICommandHandler<CreateProductCommand, CreateProductResponse>
{
    public async Task<CreateProductResponse> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        Category category = await dbContext.Categories
                                           .FirstAsync(c => c.Id == (CategoryId)command.CreateProductRequest.CategoryId, cancellationToken);

        IEnumerable<Ingredient> ingredients = await dbContext.Ingredients.GetByIds(command.CreateProductRequest.IngredientIds.Select(id => (IngredientId)id), cancellationToken);

        IEnumerable<Ingredient> customIngredients = await dbContext.Ingredients.GetByIds(command.CreateProductRequest.CustomIngredientIds.Select(id => (IngredientId)id), cancellationToken);

        Product product = command.CreateProductRequest.ToDomain(category, ingredients, customIngredients);

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateProductResponse(product.Id.Value);
    }
}
