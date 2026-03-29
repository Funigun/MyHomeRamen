using MyHomeRamen.Api.Common.Endpoint.Models;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.CreateIngredient.Models;

public sealed record CreateIngredientRequest(
    string Name,
    string Description,
    decimal Price,
    List<Guid> CategoryIds) : IRequest<Guid>;
