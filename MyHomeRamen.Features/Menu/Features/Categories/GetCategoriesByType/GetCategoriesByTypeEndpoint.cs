using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Endpoints.Query;

namespace MyHomeRamen.Features.Menu.Features.Categories.GetCategoriesByType;

public sealed record GetCategoriesByTypeRequest(int CategoryType);

public sealed record CategoryByTypeDto(Guid Id, string Name, int SortOrder);

public sealed record GetCategoriesByTypeResponse(IEnumerable<CategoryByTypeDto> Categories);

public sealed class GetCategoriesByTypeEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardGet<GetCategoriesByTypeResponse>("api/menu/categories/by-type", HandleAsync)
            .WithName("GetCategoriesByTypeEndpoint")
            .WithTags("Categories")
            .WithDescription("Returns a filtered and ordered list of categories for the specified category type.")
            .RequireAuthorization("RestaurantManager");
    }

    private static async Task<Results<Ok<GetCategoriesByTypeResponse>, ForbidHttpResult>> HandleAsync(
        [AsParameters] GetCategoriesByTypeRequest request,
        [FromServices] IQueryHandler<GetCategoriesByTypeQuery, GetCategoriesByTypeResponse> handler,
        CancellationToken cancellationToken)
    {
        GetCategoriesByTypeQuery query = new(request);
        GetCategoriesByTypeResponse response = await handler.Handle(query, cancellationToken);

        return TypedResults.Ok(response);
    }
}
