namespace MyHomeRamen.Api.Menu.Features.Products.GetProductsForManage.Models;

public sealed record ProductDto(Guid Id, string Name, string? Description, decimal Price);
