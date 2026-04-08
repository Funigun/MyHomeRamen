using MyHomeRamen.Api.Common.Endpoint.Models;

namespace MyHomeRamen.Api.Menu.Features.Products.UpdateProduct.Models;

public sealed record UpdateProductRequest(
    string Name,
    string? Description,
    decimal Price,
    Guid CategoryId,
    IEnumerable<Guid> IngredientIds,
    IEnumerable<Guid> CustomIngredientIds) : IRequest<UpdateProductResponse>
{
    public Guid Id { get; set; }
}
