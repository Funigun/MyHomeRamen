using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Products.CreateProduct.Models;
using MyHomeRamen.Api.Menu.Features.Products.CreateProduct.Models.DTOs;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Products;

namespace MyHomeRamen.Api.Menu.Features.Products.CreateProduct;

public sealed class CreateProductHandler : IRequestHandler<CreateProductRequest, Guid>
{
    private readonly IMenuDbContext _dbContext;

    public CreateProductHandler(IMenuDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> Handle(CreateProductRequest request, CancellationToken cancellationToken)
    {
        Category category = await _dbContext.Categories
            .FirstAsync(c => c.Id == (CategoryId)request.CategoryId, cancellationToken);

        List<Ingredient> ingredients = await _dbContext.Ingredients
            .Where(i => request.IngredientIds.Select(id => (IngredientId)id).Contains(i.Id))
            .ToListAsync(cancellationToken);

        Product product = request.ToDomain(category, ingredients);

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return product.Id.Value;
    }
}
