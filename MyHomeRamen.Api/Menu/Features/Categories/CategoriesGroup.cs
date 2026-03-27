using MyHomeRamen.Api.Common.Endpoint;

namespace MyHomeRamen.Api.Menu.Features.Categories;

public sealed class CategoriesGroup : IGroupEndpoint
{
    public string GroupName { get; init; } = "Menu";

    public void Configure(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.WithTags("Categories")
                    .WithDescription("Categories management operations")
                    .RequireAuthorization();
    }
}
