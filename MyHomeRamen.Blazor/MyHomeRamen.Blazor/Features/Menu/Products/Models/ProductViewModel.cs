using MyHomeRamen.Blazor.Features.Menu.Products.Shared;

namespace MyHomeRamen.Blazor.Features.Menu.Products.Models;

public sealed record ProductViewModel(Guid Id, string Name, string Description, decimal Price)
{
    public static ProductViewModel FromResponse(ProductForManageResponse response) =>
        new(response.Id, response.Name, response.Description ?? string.Empty, response.Price);
}
