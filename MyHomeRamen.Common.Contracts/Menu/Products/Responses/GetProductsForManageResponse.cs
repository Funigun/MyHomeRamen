using MyHomeRamen.Common.Contracts.Menu.Products.DTOs;

namespace MyHomeRamen.Common.Contracts.Menu.Products.Responses;

public sealed record GetProductsForManageResponse(int Page, int PageSize, int TotalCount, IEnumerable<ProductForManageDto> Products);
