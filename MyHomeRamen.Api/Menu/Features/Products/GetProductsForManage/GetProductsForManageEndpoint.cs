using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Products.GetProductsForManage.Models;
using MyHomeRamen.Api.WebPresentation;

namespace MyHomeRamen.Api.Menu.Features.Products.GetProductsForManage;

public sealed class GetProductsForManageEndpoint : IEndpoint
{
    public string GroupName { get; init; } = "Menu";

    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardValidatedGet<GetProductsForManageRequest, GetProductsForManageResponse>("products/manage", HandleAsync)
            .WithName("GetProductsForManageEndpoint")
            .WithDescription("Returns a filtered, sorted, and paged list of products for the admin management view.")
            .RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [AsParameters] GetProductsForManageRequest request,
        [AsParameters] PageParameters pageParameters,
        [FromServices] IRequestHandler<GetProductsForManageRequest, GetProductsForManageResponse> handler,
        CancellationToken cancellationToken)
    {
        request.PageParameters = pageParameters;
        GetProductsForManageResponse response = await handler.Handle(request, cancellationToken);
        return Results.Ok(response);
    }
}
