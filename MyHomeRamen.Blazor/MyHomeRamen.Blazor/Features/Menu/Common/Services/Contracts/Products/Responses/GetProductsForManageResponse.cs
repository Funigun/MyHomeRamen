using MyHomeRamen.Blazor.Features.Menu.Common.Services.Contracts.Products.DTOs;

namespace MyHomeRamen.Blazor.Features.Menu.Common.Services.Contracts.Products.Responses;

public sealed record GetProductsForManageResponse(int Page, int PageSize, int TotalCount, IEnumerable<ProductForManageDto> Products);
