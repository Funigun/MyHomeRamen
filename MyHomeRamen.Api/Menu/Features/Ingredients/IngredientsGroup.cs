using MyHomeRamen.Api.Common.Endpoint;

namespace MyHomeRamen.Api.Menu.Features.Ingredients;

public sealed class IngredientsGroup : IGroupEndpoint
{
    public string GroupName { get; init; } = "Menu";

    public void Configure(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.WithTags("Ingredients")
                    .WithDescription("Ingredients management operations")
                    .RequireAuthorization();
    }
}
