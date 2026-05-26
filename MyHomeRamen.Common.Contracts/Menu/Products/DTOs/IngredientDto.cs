namespace MyHomeRamen.Common.Contracts.Menu.Products.DTOs;

public sealed record IngredientDto(Guid Id, string Name, string Description, decimal Price);
