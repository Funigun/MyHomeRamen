namespace MyHomeRamen.Blazor.Features.Menu.Products.Responses;

public sealed record GetProductsForManageResponse(
    int Page,
    int PageSize,
    int TotalCount,
    IEnumerable<ProductForManageItemResponse> Products);
