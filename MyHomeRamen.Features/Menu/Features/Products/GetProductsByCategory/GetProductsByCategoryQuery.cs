using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Menu.Features.Abstractions;

namespace MyHomeRamen.Features.Menu.Features.Products.GetProductsByCategory;

public sealed record GetProductsByCategoryQuery(GetProductsByCategoryRequest Request) : IQuery<GetProductsByCategoryResponse>;

public sealed record GetProductsByCategoryQueryOptions(CategoryId CategoryId)
                   : DbQueryOptions<Product, ProductByCategoryDto>
                   (
                       new()
                       {
                           Filter = product => product.Categories.Any(category => category.Id == CategoryId),
                           OrderBy = product => product.Name,
                           OrderDirection = "asc",
                           Selector = product => new ProductByCategoryDto(
                               product.Id.Value,
                               product.Name,
                               product.Description,
                               product.Price,
                               product.ImageUrl,
                               product.BaseIngredients.Select(ingredient => new ProductIngredientDto(ingredient.Id.Value, ingredient.Name)).ToList())
                       }
                   );

public sealed class GetProductsByCategoryHandler(IMenuDbContext dbContext) : IQueryHandler<GetProductsByCategoryQuery, GetProductsByCategoryResponse>
{
    public async Task<GetProductsByCategoryResponse> Handle(GetProductsByCategoryQuery query, CancellationToken cancellationToken)
    {
        GetProductsByCategoryQueryOptions options = new(new CategoryId(query.Request.CategoryId));

        IEnumerable<ProductByCategoryDto> products = await dbContext.Product.Query().GetByCategory(options, cancellationToken);

        return new GetProductsByCategoryResponse(products);
    }
}
