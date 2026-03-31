using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesForManage.Models;
using MyHomeRamen.Api.WebPresentation;

namespace MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesForManage;

public sealed class GetCategoriesForManageEndpoint : IEndpoint
{
    public string GroupName { get; init; } = "Menu";

    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardGet<GetCategoriesForManageResponse>("categories/manage", HandleAsync)
            .WithName("GetCategoriesForManageEndpoint")
            .WithDescription("Returns all categories grouped by type for admin management.")
            .RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] IRequestHandler<GetCategoriesForManageRequest, GetCategoriesForManageResponse> handler,
        CancellationToken cancellationToken)
    {
        GetCategoriesForManageResponse response = await handler.Handle(
            new GetCategoriesForManageRequest(), cancellationToken);
        return Results.Ok(response);
    }
}
