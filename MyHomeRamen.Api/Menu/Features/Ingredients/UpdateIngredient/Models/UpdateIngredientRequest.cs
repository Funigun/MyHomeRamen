using MyHomeRamen.Api.Common.Endpoint.Models;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.UpdateIngredient.Models;

public sealed record UpdateIngredientRequest(
    string Name,
    string Description,
    decimal Price,
    IEnumerable<Guid> CategoryIds) : IRequest<UpdateIngredientResponse>
{
    [RouteParam]
    public Guid Id { get; init; }
}
