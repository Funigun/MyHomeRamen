using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Api.WebPresentation;
using MyHomeRamen.Common.Contracts.Menu.Products.Responses;

namespace MyHomeRamen.Api.Menu.Features.Products.GetProductByIdForManage;

public sealed class GetProductByIdForManageEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardGet<GetProductByIdForManageResponse>("api/menu/products/{id}/manage", HandleAsync)
            .WithName("GetProductByIdForManageEndpoint")
            .WithTags("Products")
            .WithDescription("Returns the full details of a single product by its ID for the management view using the /manage route.")
            .RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        [FromServices] IQueryHandler<GetProductByIdForManageQuery, GetProductByIdForManageResponse> handler,
        CancellationToken cancellationToken)
    {
        GetProductByIdForManageQuery query = new(id);
        GetProductByIdForManageResponse response = await handler.Handle(query, cancellationToken);

        return Results.Ok(response);
    }
}
