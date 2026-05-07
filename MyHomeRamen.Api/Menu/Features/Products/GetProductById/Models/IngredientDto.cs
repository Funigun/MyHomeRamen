namespace MyHomeRamen.Api.Menu.Features.Products.GetProductById.Models;

public sealed record IngredientDto(Guid Id, string Name, string Description, decimal Price);
