using MyHomeRamen.Api.Common.Endpoint.Models;

namespace MyHomeRamen.Api.Menu.Features.Products.GetProductsForManage.Models;

public sealed record GetProductsForManageRequest(
    string? Name,
    IEnumerable<Guid>? CategoryIds,
    IEnumerable<Guid>? IngredientIds,
    decimal? PriceFrom,
    decimal? PriceTo,
    string? OrderBy) : IRequest<GetProductsForManageResponse>
{
    public PageParameters PageParameters { get; set; }
}
