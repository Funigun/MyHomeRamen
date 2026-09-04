using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http.HttpResults;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Menu.Features.Categories.GetMenuCategories;

public sealed record CategoryForMenuDto(Guid Id, string Name);

public sealed record GetMenuCategoriesResponse(IEnumerable<CategoryForMenuDto> Categories);

public sealed class GetMenuCategoriesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardGet<GetMenuCategoriesResponse>("api/menu/categories/menu", HandleAsync)
            .WithName("GetMenuCategoriesEndpoint")
            .WithTags("Categories")
            .WithDescription("Returns all product categories for the public restaurant menu page.")
            .AllowAnonymous();
    }

    private static async Task<Results<Ok<GetMenuCategoriesResponse>, BadRequest>> HandleAsync(
        [FromServices] IRequestHandler<GetMenuCategoriesQuery, GetMenuCategoriesResponse> handler,
        CancellationToken cancellationToken)
    {
        GetMenuCategoriesQuery query = new();
        GetMenuCategoriesResponse response = await handler.Handle(query, cancellationToken);

        return TypedResults.Ok(response);
    }
}
