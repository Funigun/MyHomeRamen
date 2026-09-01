using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Features.Common.Endpoints.Models;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.GetIngredientsForManage;

public sealed record GetIngredientsForManageRequest(string? Name, Guid[]? CategoryIds);

public sealed record IngredientForManageDto(Guid Id, string Name, string Description);

public sealed record GetIngredientsForManageResponse(int Page, int PageSize, int TotalCount, IEnumerable<IngredientForManageDto> Ingredients);

public sealed class GetIngredientsForManageEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardGet<GetIngredientsForManageResponse>("api/menu/ingredients/manage", HandleAsync)
            .WithName("GetIngredientsForManageEndpoint")
            .WithTags("Ingredients")
            .WithDescription("Returns a filtered list of ingredients for the admin management view. Supports optional name (contains, case-insensitive) and category ID filters.")
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedUserPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [AsParameters] GetIngredientsForManageRequest request,
        [AsParameters] PageParameters pageParameters,
        [FromServices] IRequestHandler<GetIngredientsForManageQuery, GetIngredientsForManageResponse> handler,
        CancellationToken cancellationToken)
    {
        GetIngredientsForManageQuery query = new(request, pageParameters);
        GetIngredientsForManageResponse response = await handler.Handle(query, cancellationToken);

        return Results.Ok(response);
    }
}
