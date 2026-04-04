using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesByType.Models;
using MyHomeRamen.Api.WebPresentation;

namespace MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesByType;

public sealed class GetCategoriesByTypeEndpoint : IEndpoint
{
    public string GroupName { get; init; } = "Menu";

    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardValidatedGet<GetCategoriesByTypeRequest, IEnumerable<GetCategoriesByTypeResponse>>("categories/by-type", HandleAsync)
            .WithName("GetCategoriesByTypeEndpoint")
            .WithDescription("Returns a filtered and ordered list of categories for the specified category type.")
            .RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [AsParameters] GetCategoriesByTypeRequest request,
        [FromServices] IRequestHandler<GetCategoriesByTypeRequest, IEnumerable<GetCategoriesByTypeResponse>> handler,
        CancellationToken cancellationToken)
    {
        IEnumerable<GetCategoriesByTypeResponse> response = await handler.Handle(request, cancellationToken);
        return Results.Ok(response);
    }
}
