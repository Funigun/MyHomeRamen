using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Api.WebPresentation;
using MyHomeRamen.Common.Contracts.Menu.Products.Requests;
using MyHomeRamen.Common.Contracts.Menu.Products.Responses;

namespace MyHomeRamen.Api.Menu.Features.Products.GetProductsForManage;

public sealed class GetProductsForManageEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardGet<GetProductsForManageResponse>("api/menu/products/manage", HandleAsync)
            .WithName("GetProductsForManageEndpoint")
            .WithTags("Products")
            .WithDescription("Returns a filtered, sorted, and paged list of products for the admin management view.")
            .RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [AsParameters] GetProductsForManageRequest request,
        [AsParameters] PageParameters pageParameters,
        [FromServices] IQueryHandler<GetProductsForManageQuery, GetProductsForManageResponse> handler,
        CancellationToken cancellationToken)
    {
        GetProductsForManageQuery query = new(pageParameters, request);
        GetProductsForManageResponse response = await handler.Handle(query, cancellationToken);
        
        return Results.Ok(response);
    }
}
