namespace MyHomeRamen.Blazor.Features.Menu.Common.Services.Contracts.Products.Responses;

public sealed record GetProductByIdForManageResponse(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    Guid CategoryId,
    IEnumerable<Guid> IngredientIds,
    IEnumerable<Guid> CustomIngredientIds);
