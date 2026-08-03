using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Features.Common.Endpoints;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.GetIngredientById;

public sealed record GetIngredientByIdResponse(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    IEnumerable<Guid> CategoryIds);

public sealed class GetIngredientByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardGet<GetIngredientByIdResponse>("api/menu/ingredients/{id}", HandleAsync)
            .WithName("GetIngredientByIdEndpoint")
            .WithTags("Ingredients")
            .WithDescription("Returns the full details of a single ingredient by its ID.")
            .RequireAuthorization("RestaurantManager");
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        [FromServices] IQueryHandler<GetIngredientByIdQuery, GetIngredientByIdResponse> handler,
        CancellationToken cancellationToken)
    {
        GetIngredientByIdQuery query = new(id);
        GetIngredientByIdResponse response = await handler.Handle(query, cancellationToken);
        return Results.Ok(response);
    }
}

