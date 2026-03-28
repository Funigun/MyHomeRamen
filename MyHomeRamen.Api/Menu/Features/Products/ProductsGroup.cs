using MyHomeRamen.Api.Common.Endpoint;

namespace MyHomeRamen.Api.Menu.Features.Products;

public sealed class ProductsGroup : IGroupEndpoint
{
    public string GroupName { get; init; } = "Menu";

    public void Configure(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.WithTags("Products")
                    .WithDescription("Products management operations")
                    .RequireAuthorization();
    }
}
