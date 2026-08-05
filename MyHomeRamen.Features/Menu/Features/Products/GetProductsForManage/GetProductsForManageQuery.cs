using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Features.Common.Endpoints.Models;
using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Menu.Features.Abstractions;

namespace MyHomeRamen.Features.Menu.Features.Products.GetProductsForManage;

public sealed record GetProductsForManageQuery(PageParameters PageParameters, GetProductsForManageRequest Request) : IQuery<GetProductsForManageResponse>;

public sealed record GetProductsForManageQueryOptions(GetProductsForManageRequest Request, PageParameters PageParameters)
    : PagedDbQueryOptions<Product, ProductForManageDto>
    (
        new()
        {
            Filter = product =>
                (string.IsNullOrWhiteSpace(Request.Name) || product.Name.ToLower().Contains(Request.Name.ToLower())) &&
                (Request.CategoryIds == null || product.Categories.Any(category => Request.CategoryIds.Contains(category.Id.Value))) &&
                (Request.IngredientIds == null || product.BaseIngredients.Any(ingredient => Request.IngredientIds.Contains(ingredient.Id.Value)) || product.CustomIngredients.Any(ingredient => Request.IngredientIds.Contains(ingredient.Id.Value))) &&
                (Request.PriceFrom == null || product.Price >= Request.PriceFrom.Value) &&
                (Request.PriceTo == null || product.Price <= Request.PriceTo.Value),
            OrderBy = Request.OrderBy switch
            {
                "Price" => product => product.Price,
                _ => product => product.Name
            },
            OrderDirection = "asc",
            PageNumber = PageParameters.PageNumber,
            PageSize = PageParameters.PageSize,
            Selector = product => new ProductForManageDto(product.Id.Value, product.Name, product.Description, product.Price)
        }
    );

public sealed class GetProductsForManageHandler(IMenuDbContext dbContext) : IQueryHandler<GetProductsForManageQuery, GetProductsForManageResponse>
{
    public async Task<GetProductsForManageResponse> Handle(
        GetProductsForManageQuery query,
        CancellationToken cancellationToken)
    {
        GetProductsForManageQueryOptions options = new(query.Request, query.PageParameters);

        PagedResult<ProductForManageDto> pagedResult = await dbContext.Product.Query().ForManage(options, cancellationToken);

        return new GetProductsForManageResponse(
            Page: query.PageParameters.PageNumber,
            PageSize: query.PageParameters.PageSize,
            TotalCount: pagedResult.TotalItems,
            Products: pagedResult.Items);
    }
}

