using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Api.WebPresentation;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Requests;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientsForManage;

public sealed class GetIngredientsForManageEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardGet<GetIngredientsForManageResponse>("api/menu/ingredients/manage", HandleAsync)
            .WithName("GetIngredientsForManageEndpoint")
            .WithTags("Ingredients")
            .WithDescription("Returns a filtered list of ingredients for the admin management view. Supports optional name (contains, case-insensitive) and category ID filters.")
            .RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [AsParameters] GetIngredientsForManageRequest request,
        [AsParameters] PageParameters pageParameters,
        [FromServices] IQueryHandler<GetIngredientsForManageQuery, GetIngredientsForManageResponse> handler,
        CancellationToken cancellationToken)
    {
        GetIngredientsForManageQuery query = new(request) { PageParameters = pageParameters };
        GetIngredientsForManageResponse response = await handler.Handle(query, cancellationToken);
        return Results.Ok(response);
    }
}
