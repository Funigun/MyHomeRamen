using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Common.Contracts.Menu.Categories.Responses;
using MyHomeRamen.Features.Common.Endpoints;

namespace MyHomeRamen.Api.Menu.Features.Categories.GetMenuCategories;

public sealed class GetMenuCategoriesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardGet<IEnumerable<GetMenuCategoriesResponse>>("api/menu/categories/menu", HandleAsync)
            .WithName("GetMenuCategoriesEndpoint")
            .WithTags("Categories")
            .WithDescription("Returns all product categories for the public restaurant menu page.")
            .AllowAnonymous();
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] IQueryHandler<GetMenuCategoriesQuery, IEnumerable<GetMenuCategoriesResponse>> handler,
        CancellationToken cancellationToken)
    {
        GetMenuCategoriesQuery query = new();
        IEnumerable<GetMenuCategoriesResponse> response = await handler.Handle(query, cancellationToken);

        return Results.Ok(response);
    }
}
