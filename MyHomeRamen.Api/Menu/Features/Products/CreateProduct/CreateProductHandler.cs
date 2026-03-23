using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Products.CreateProduct.Models;
using MyHomeRamen.Api.Menu.Features.Products.CreateProduct.Models.DTOs;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Products.CreateProduct;

public sealed class CreateProductHandler(IMenuDbContext dbContext) : IRequestHandler<CreateProductRequest, Guid>
{
    public async Task<Guid> Handle(CreateProductRequest request, CancellationToken cancellationToken)
    {
        Category category = await dbContext.Categories
                                           .FirstAsync(c => c.Id == (CategoryId)request.CategoryId, cancellationToken);

        IEnumerable<Ingredient> ingredients = await dbContext.Ingredients.GetByIds(request.IngredientIds.Select(id => (IngredientId)id), cancellationToken);

        Product product = request.ToDomain(category, ingredients);

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(cancellationToken);

        return product.Id.Value;
    }
}
