using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Api.WebPresentation;
using MyHomeRamen.Common.Contracts.Menu.Categories.Responses;

namespace MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesByType;

public sealed class GetCategoriesByTypeEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardValidatedGet<GetCategoriesByTypeQuery, IEnumerable<GetCategoriesByTypeQuery>>("api/menu/categories/by-type", HandleAsync)
            .WithName("GetCategoriesByTypeEndpoint")
            .WithTags("Categories")
            .WithDescription("Returns a filtered and ordered list of categories for the specified category type.")
            .RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [AsParameters] GetCategoriesByTypeQuery request,
        [FromServices] IQueryHandler<GetCategoriesByTypeQuery, IEnumerable<GetCategoriesByTypeResponse>> handler,
        CancellationToken cancellationToken)
    {
        IEnumerable<GetCategoriesByTypeResponse> response = await handler.Handle(request, cancellationToken);
        return Results.Ok(response);
    }
}
