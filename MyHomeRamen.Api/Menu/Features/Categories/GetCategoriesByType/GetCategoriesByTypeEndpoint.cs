using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Api.WebPresentation;
using MyHomeRamen.Common.Contracts.Menu.Categories.Requests;
using MyHomeRamen.Common.Contracts.Menu.Categories.Responses;
using MyHomeRamen.Features.Common.Endpoints;

namespace MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesByType;

public sealed class GetCategoriesByTypeEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardGet<IEnumerable<GetCategoriesByTypeQuery>>("api/menu/categories/by-type", HandleAsync)
            .WithName("GetCategoriesByTypeEndpoint")
            .WithTags("Categories")
            .WithDescription("Returns a filtered and ordered list of categories for the specified category type.")
            .RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [AsParameters] GetCategoriesByTypeRequest request,
        [FromServices] IQueryHandler<GetCategoriesByTypeQuery, IEnumerable<GetCategoriesByTypeResponse>> handler,
        CancellationToken cancellationToken)
    {
        GetCategoriesByTypeQuery query = new(request.CategoryType);
        IEnumerable<GetCategoriesByTypeResponse> response = await handler.Handle(query, cancellationToken);

        return Results.Ok(response);
    }
}
