using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Common.Contracts.Menu.Products.Responses;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Products.UpdateProduct;

public sealed class UpdateProductHandler(IMenuDbContext dbContext) : ICommandHandler<UpdateProductCommand, UpdateProductResponse>
{
    public async Task<UpdateProductResponse> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        Product product = await dbContext.Products
            .Include(p => p.Categories)
            .Include(p => p.BaseIngredients)
            .Include(p => p.CustomIngredients)
            .AsSplitQuery()
            .GetById(command.Id, cancellationToken);

        Category category = await dbContext.Categories
            .FirstAsync(c => c.Id == (CategoryId)command.UpdateProductRequest.CategoryId, cancellationToken);

        IEnumerable<Ingredient> ingredients = await dbContext.Ingredients
            .GetByIds(command.UpdateProductRequest.IngredientIds.Select(id => (IngredientId)id), cancellationToken);

        IEnumerable<Ingredient> customIngredients = await dbContext.Ingredients
            .GetByIds(command.UpdateProductRequest.CustomIngredientIds.Select(id => (IngredientId)id), cancellationToken);

        product.Update(command.UpdateProductRequest.Name, command.UpdateProductRequest.Description ?? string.Empty, command.UpdateProductRequest.Price, category, ingredients, customIngredients);

        await dbContext.SaveChangesAsync(cancellationToken);

        return product.ToResponse();
    }
}
