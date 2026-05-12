using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Products.GetProductByIdForManage.Models;
using MyHomeRamen.Api.WebPresentation;

namespace MyHomeRamen.Api.Menu.Features.Products.GetProductByIdForManage;

public sealed class GetProductByIdForManageEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardValidatedGet<GetProductByIdForManageRequest, GetProductByIdForManageResponse>("api/menu/products/{id}/manage", HandleAsync)
            .WithName("GetProductByIdForManageEndpoint")
            .WithTags("Products")
            .WithDescription("Returns the full details of a single product by its ID for the management view using the /manage route.")
            .RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy);
    }

    private static async Task<IResult> HandleAsync(
        GetProductByIdForManageRequest id,
        [FromServices] IRequestHandler<GetProductByIdForManageRequest, GetProductByIdForManageResponse> handler,
        CancellationToken cancellationToken)
    {
        GetProductByIdForManageResponse response = await handler.Handle(id, cancellationToken);
        return Results.Ok(response);
    }
}
