using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Categories.GetMenuCategories.Models;

namespace MyHomeRamen.Api.Menu.Features.Categories.GetMenuCategories;

public sealed class GetMenuCategoriesEndpoint : IEndpoint
{
    public string GroupName { get; init; } = "Menu";

    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardGet<IEnumerable<GetMenuCategoriesResponse>>("categories/menu", HandleAsync)
            .WithName("GetMenuCategoriesEndpoint")
            .WithDescription("Returns all product categories for the public restaurant menu page.")
            .AllowAnonymous();
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] IRequestHandler<GetMenuCategoriesRequest, IEnumerable<GetMenuCategoriesResponse>> handler,
        CancellationToken cancellationToken)
    {
        IEnumerable<GetMenuCategoriesResponse> response = await handler.Handle(new GetMenuCategoriesRequest(), cancellationToken);
        return Results.Ok(response);
    }
}
