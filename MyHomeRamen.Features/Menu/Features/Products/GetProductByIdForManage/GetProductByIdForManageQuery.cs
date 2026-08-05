using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Menu.Features.Abstractions;

namespace MyHomeRamen.Features.Menu.Features.Products.GetProductByIdForManage;

public record GetProductByIdForManageQuery(Guid Id) : IQuery<GetProductByIdForManageResponse>;

public sealed record GetProductByIdForManageQueryOptions(ProductId ProductId)
    : DbQueryOptions<Product, ProductByIdForManageDto>
    (
        new()
        {
            Filter = product => product.Id == ProductId,
            Selector = product => new ProductByIdForManageDto(
                product.Id.Value,
                product.Name,
                product.Description,
                product.Price,
                product.Categories[0].Id.Value,
                product.BaseIngredients.Select(ingredient => ingredient.Id.Value).ToList(),
                product.CustomIngredients.Select(ingredient => ingredient.Id.Value).ToList())
        }
    );

public sealed class GetProductByIdForManageHandler(IMenuDbContext dbContext) : IQueryHandler<GetProductByIdForManageQuery, GetProductByIdForManageResponse>
{
    public async Task<GetProductByIdForManageResponse> Handle(GetProductByIdForManageQuery query, CancellationToken cancellationToken)
    {
        ProductByIdForManageDto? product = await dbContext.Product.Query().GetByIdForManage(new GetProductByIdForManageQueryOptions((ProductId)query.Id), cancellationToken);

        return product is null
            ? throw new InvalidOperationException("Product was not found.")
            : ToResponse(product);
    }

    private static GetProductByIdForManageResponse ToResponse(ProductByIdForManageDto product)
        => new(product.Id, product.Name, product.Description, product.Price, product.CategoryId, product.IngredientIds, product.CustomIngredientIds);
}
