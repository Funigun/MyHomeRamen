namespace MyHomeRamen.Blazor.Features.Menu.Common.Services.Contracts.Ingredients.Responses;

public sealed record GetIngredientByIdResponse(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    IEnumerable<Guid> CategoryIds);
