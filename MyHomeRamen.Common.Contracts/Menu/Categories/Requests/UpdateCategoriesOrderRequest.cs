using MyHomeRamen.Common.Contracts.Menu.Categories.DTOs;

namespace MyHomeRamen.Common.Contracts.Menu.Categories.Requests;

public sealed record UpdateCategoriesOrderRequest(IEnumerable<CategoryOrderItemDto> Items);
