namespace MyHomeRamen.Common.Contracts.Menu.Products.DTOs;

public sealed record ProductForManageDto(Guid Id, string Name, string? Description, decimal Price);
