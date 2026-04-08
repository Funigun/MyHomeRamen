namespace MyHomeRamen.Api.Menu.Features.Products.GetProductsForManage.Models;

public sealed record GetProductsForManageResponse(int Page, int PageSize, int TotalCount, IEnumerable<ProductDto> Products);
