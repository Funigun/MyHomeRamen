using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Authorization;

namespace MyHomeRamen.Features.Menu.Features.Products.GetProductByIdForManage;

public sealed record ProductByIdForManageDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    Guid CategoryId,
    IEnumerable<Guid> IngredientIds,
    IEnumerable<Guid> CustomIngredientIds);

public sealed record GetProductByIdForManageResponse(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    Guid CategoryId,
    IEnumerable<Guid> IngredientIds,
    IEnumerable<Guid> CustomIngredientIds);

public sealed class GetProductByIdForManageEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardGet<GetProductByIdForManageResponse>("api/menu/products/{id}/manage", HandleAsync)
            .WithName("GetProductByIdForManageEndpoint")
            .WithTags("Products")
            .WithDescription("Returns the full details of a single product by its ID for the management view using the /manage route.")
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedUserPolicy);
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

