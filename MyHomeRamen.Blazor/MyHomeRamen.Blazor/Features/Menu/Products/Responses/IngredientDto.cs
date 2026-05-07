namespace MyHomeRamen.Blazor.Features.Menu.Products.Responses;

public sealed record IngredientDto(Guid Id, string Name, string Description, decimal Price);
