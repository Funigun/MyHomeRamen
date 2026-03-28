using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Api.Common.Endpoint;

namespace MyHomeRamen.Api.Menu.Features.GetCategoriesOptions;

public sealed class GetCategoriesOptionsEndpoint : IEndpoint
{
    public string GroupName { get; init; } = "Menu";

    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("categories/options", async (ICategoryService categoryService, CancellationToken cancellationToken) =>
        {
            IReadOnlyCollection<CategoryOption> options = await categoryService.GetCategoriesOptionsAsync(cancellationToken);

            return Results.Ok(options);
        })
        .WithName("GetCategoriesOptions")
        .WithTags("Menu")
        .Produces<IReadOnlyCollection<CategoryOption>>();
    }
}
