using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientsForManage.Models;
using MyHomeRamen.Api.WebPresentation;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientsForManage;

public sealed class GetIngredientsForManageEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardValidatedGet<GetIngredientsForManageRequest, GetIngredientsForManageResponse>("api/menu/ingredients/manage", HandleAsync)
            .WithName("GetIngredientsForManageEndpoint")
            .WithTags("Ingredients")
            .WithDescription("Returns a filtered list of ingredients for the admin management view. Supports optional name (contains, case-insensitive) and category ID filters.")
            .RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [AsParameters] GetIngredientsForManageRequest request,
        [AsParameters] PageParameters pageParameters,
        [FromServices] IRequestHandler<GetIngredientsForManageRequest, GetIngredientsForManageResponse> handler,
        CancellationToken cancellationToken)
    {
        request.PageParameters = pageParameters;
        GetIngredientsForManageResponse response = await handler.Handle(request, cancellationToken);
        return Results.Ok(response);
    }
}
