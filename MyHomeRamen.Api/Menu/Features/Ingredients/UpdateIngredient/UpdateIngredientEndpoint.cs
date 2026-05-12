using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Ingredients.UpdateIngredient.Models;
using MyHomeRamen.Api.WebPresentation;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.UpdateIngredient;

public sealed class UpdateIngredientEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardValidatedPutWithResponse<UpdateIngredientRequest, UpdateIngredientResponse>(
                "api/menu/ingredients/{id}", HandleAsync)
            .WithName("UpdateIngredientEndpoint")
            .WithTags("Ingredients")
            .WithDescription("Updates the name, description, price, and categories of an existing ingredient.")
            .RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] UpdateIngredientIRequestId id,
        [FromBody] UpdateIngredientRequest request,
        [FromServices] IRequestHandler<UpdateIngredientRequest, UpdateIngredientResponse> handler,
        CancellationToken cancellationToken)
    {
        UpdateIngredientResponse response = await handler.Handle(request with { Id = id.Id }, cancellationToken);

        return Results.Ok(response);
    }
}
