using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Requests;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Endpoints.Models;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.GetIngredientsForManage;

public sealed class GetIngredientsForManageEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardGet<GetIngredientsForManageResponse>("api/menu/ingredients/manage", HandleAsync)
            .WithName("GetIngredientsForManageEndpoint")
            .WithTags("Ingredients")
            .WithDescription("Returns a filtered list of ingredients for the admin management view. Supports optional name (contains, case-insensitive) and category ID filters.")
            .RequireAuthorization("RestaurantManager");
    }

    private static async Task<IResult> HandleAsync(
        [AsParameters] GetIngredientsForManageRequest request,
        [AsParameters] PageParameters pageParameters,
        [FromServices] IQueryHandler<GetIngredientsForManageQuery, GetIngredientsForManageResponse> handler,
        CancellationToken cancellationToken)
    {
        GetIngredientsForManageQuery query = new(request, pageParameters);
        GetIngredientsForManageResponse response = await handler.Handle(query, cancellationToken);

        return Results.Ok(response);
    }
}
