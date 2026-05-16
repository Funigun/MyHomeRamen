using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Api.WebPresentation;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientById;

public sealed class GetIngredientByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardGet<GetIngredientByIdResponse>("api/menu/ingredients/{id}", HandleAsync)
            .WithName("GetIngredientByIdEndpoint")
            .WithTags("Ingredients")
            .WithDescription("Returns the full details of a single ingredient by its ID.")
            .RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy);
    }

    private static async Task<IResult> HandleAsync(
        GetIngredientByIdQuery id,
        [FromServices] IQueryHandler<GetIngredientByIdQuery, GetIngredientByIdResponse> handler,
        CancellationToken cancellationToken)
    {
        GetIngredientByIdResponse response = await handler.Handle(id, cancellationToken);
        return Results.Ok(response);
    }
}
