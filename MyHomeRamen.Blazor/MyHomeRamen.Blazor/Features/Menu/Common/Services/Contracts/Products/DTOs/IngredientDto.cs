namespace MyHomeRamen.Blazor.Features.Menu.Common.Services.Contracts.Products.DTOs;

public sealed record IngredientDto(Guid Id, string Name, string Description, decimal Price);
