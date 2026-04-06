using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Ingredients.UpdateIngredient.Models;
using MyHomeRamen.Api.WebPresentation;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.UpdateIngredient;

public sealed class UpdateIngredientEndpoint : IEndpoint
{
    public string GroupName { get; init; } = "Menu";

    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardValidatedPutWithResponse<UpdateIngredientRequest, UpdateIngredientResponse>(
                "ingredients/{id}", HandleAsync)
            .WithName("UpdateIngredientEndpoint")
            .WithDescription("Updates the name, description, price, and categories of an existing ingredient.")
            .RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] UpdateIngredientRequest request,
        [FromServices] IRequestHandler<UpdateIngredientRequest, UpdateIngredientResponse> handler,
        CancellationToken cancellationToken)
    {
        UpdateIngredientResponse response = await handler.Handle(request, cancellationToken);
        return Results.Ok(response);
    }
}
