namespace MyHomeRamen.Blazor.Features.Menu.Categories.Requests;

public sealed record UpdateCategoriesOrderRequest(IEnumerable<CategoryOrderItem> Items);
