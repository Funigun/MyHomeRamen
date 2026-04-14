using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Products.UpdateProduct.Models;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Products.UpdateProduct;

public sealed class UpdateProductHandler(IMenuDbContext dbContext) : IRequestHandler<UpdateProductRequest, UpdateProductResponse>
{
    public async Task<UpdateProductResponse> Handle(UpdateProductRequest request, CancellationToken cancellationToken)
    {
        Product product = await dbContext.Products
            .Include(p => p.Categories)
            .Include(p => p.BaseIngredients)
            .Include(p => p.CustomIngredients)
            .AsSplitQuery()
            .GetById((ProductId)request.Id, cancellationToken);

        Category category = await dbContext.Categories
            .FirstAsync(c => c.Id == (CategoryId)request.CategoryId, cancellationToken);

        IEnumerable<Ingredient> ingredients = await dbContext.Ingredients
            .GetByIds(request.IngredientIds.Select(id => (IngredientId)id), cancellationToken);

        IEnumerable<Ingredient> customIngredients = await dbContext.Ingredients
            .GetByIds(request.CustomIngredientIds.Select(id => (IngredientId)id), cancellationToken);

        product.Update(request.Name, request.Description ?? string.Empty, request.Price, category, ingredients, customIngredients);

        await dbContext.SaveChangesAsync(cancellationToken);

        return product.ToResponse();
    }
}
