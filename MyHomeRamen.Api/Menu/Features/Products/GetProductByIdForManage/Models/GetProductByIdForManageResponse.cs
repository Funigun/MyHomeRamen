namespace MyHomeRamen.Api.Menu.Features.Products.GetProductByIdForManage.Models;

public sealed record GetProductByIdForManageResponse(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    Guid CategoryId,
    IEnumerable<Guid> IngredientIds,
    IEnumerable<Guid> CustomIngredientIds);
