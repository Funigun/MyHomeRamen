namespace MyHomeRamen.Blazor.Features.Menu.Categories.UpdateCategoriesOrder;

public sealed record CategoryOrderItem(Guid Id, int SortOrder);

public sealed record UpdateCategoriesOrderRequest(List<CategoryOrderItem> Items);
