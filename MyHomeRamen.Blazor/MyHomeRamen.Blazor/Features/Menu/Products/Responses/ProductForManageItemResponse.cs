namespace MyHomeRamen.Blazor.Features.Menu.Products.Responses;

public sealed record ProductForManageItemResponse(Guid Id, string Name, string? Description, decimal Price);
