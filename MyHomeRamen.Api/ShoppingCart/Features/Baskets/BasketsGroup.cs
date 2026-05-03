using MyHomeRamen.Api.Common.Endpoint;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets;

public sealed class BasketsGroup : IGroupEndpoint
{
    public string GroupName { get; init; } = "ShoppingCart";

    public void Configure(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.WithTags("Baskets")
                    .WithDescription("Basket operations for the current user.")
                    .RequireAuthorization();
    }
}
