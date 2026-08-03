using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Features.Menu.Features.Abstractions;

namespace MyHomeRamen.Features.Menu.Features.Products.GetProductsByCategory;

public sealed record GetProductsByCategoryQuery(GetProductsByCategoryRequest Request) : IQuery<IEnumerable<GetProductsByCategoryResponse>>;

public sealed class GetProductsByCategoryHandler(IMenuDbContext dbContext) : IQueryHandler<GetProductsByCategoryQuery, IEnumerable<GetProductsByCategoryResponse>>
{
    public async Task<IEnumerable<GetProductsByCategoryResponse>> Handle(GetProductsByCategoryQuery query, CancellationToken cancellationToken)
    {
        List<Product> products = await dbContext.Product.Query()
                                                        .GetByCategory(new CategoryId(query.Request.CategoryId), cancellationToken);

        return products.Select(p => p.ToResponse());
    }
}
