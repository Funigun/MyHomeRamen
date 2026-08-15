using MyHomeRamen.Blazor.Features.Menu.Common.Services.Contracts.Categories.DTOs;

namespace MyHomeRamen.Blazor.Features.Menu.Common.Services.Contracts.Categories.Requests;

public sealed record UpdateCategoriesOrderRequest(IEnumerable<CategoryOrderItemDto> Items);
